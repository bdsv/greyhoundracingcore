package Dogs::HTMLParser;

use strict;
use warnings;

use Data::Dumper;
use LWP::UserAgent;
use HTML::TreeBuilder::XPath;

my $months = {
	'JAN' => '01', 
	'FEB' => '02', 
	'MAR' => '03',
	'APR' => '04',
	'MAY' => '05',
	'JUN' => '06',
	'JUL' => '07',
	'AUG' => '08',
	'SEP' => '09', 
	'OCT' => '10', 
	'NOV' => '11',
	'DEC' => '12'
};

sub get_request {
	my ($url) = @_;
 
	my $ua = LWP::UserAgent->new;
	$ua->timeout(10);
	$ua->env_proxy;
 
	my $response = $ua->get($url);
 
	if ($response->is_success) {
		return $response;
	}
	
	return undef;
}
	
sub get_dog_info {
	my ($response, $dog_name) = @_;	
	
	print "Getting info about dog $dog_name ...\n";
	
	my $content = $response->content if( $response );
	
	return if not defined $content;
	my $rand_number = int(rand(999999999));
	my $random_file = "tmp_$rand_number.html";
	open(my $FH, ">", $random_file) or die "Unable to create file $random_file";
	print $FH $content;
	close($FH);

	my $dog_info = ();
	my $tree= HTML::TreeBuilder::XPath->new;	
	eval {		
		$tree->parse_file($random_file);
		# 
		my $info_node = $tree->findnodes('/html/body//div[@class="popUp popUpLight popUpDog"]/div[@class="popUpHead"]')->[0];
		my $name = $info_node->findvalue('./h1');

		#$tree->delete and die "Invalid greyhound name found." if( uc($name) ne uc($dog_name) );
		
		my $li_node = $info_node->findnodes('./ul/li')->[0];
		my $li_value = $li_node->findvalue('.');
		# (11 Jul 2007) bk d 
		if( $li_value =~ m/^\s*\((\d+)\s+(\w+)\s+(\d+)\)/ ) {
			my $day = $1; if( $day < 10 ) { $day = '0' . $day; }
			my $month = $months->{uc($2)};
			my $year = $3;
			$dog_info->{birth_date} = "$day-$month-$year";
		}
	};	
	unlink($random_file);
	
	if( $@ ) {
		print "Exception: $@";
	}	
	
	return $dog_info;
}

sub parse_html_day_races_results_table {
	my ($dogs, $html_file) = @_;
	
	my $dayraces = ();
		
	open(my $FH2, ">>", "races_processed.txt") or exit;
	print $FH2 "Processing file [$html_file] .........  ";
	
	my $tree= HTML::TreeBuilder::XPath->new;	
	eval {		
		$tree->parse_file($html_file);
	
		my $tabBlock = $tree->findnodes('/html/body//div[@class="tabBlock"]')->[0];
		my $tabContent = $tabBlock->findnodes('//div[@class="tabContent tabCube"]')->[0];
		my @fullResults = $tabContent->findnodes('//div[@class="fullResults "]');
		# Add the last node
		push(@fullResults, $tabContent->findnodes('//div[@class="fullResults fullResultsEnd"]'));
		
		my $idx_span = 0;
		foreach my $resultTable ( @fullResults ) {
			# h4 has the information about the Grade the Distance of the race and the Type of surface (Flat / Obstacles)
			my $dogHeadTextH4 = $resultTable->findvalue('./div[@class="dogHeadText"]/h4');
			#h5 has the hour the race took place - it will be used as the key of the races hash
			my $dogHeadTextH5 = $resultTable->findvalue('./div[@class="dogHeadText"]/h5');
			# A6 480m Flat
			if( $dogHeadTextH4 =~ m/^\s*(\w+)\s*(\d+)m\s(\w+)\s*$/ ) {
				$dayraces->{$dogHeadTextH5}->{grade} = $1;
				$dayraces->{$dogHeadTextH5}->{distance} = $2;
				$dayraces->{$dogHeadTextH5}->{race_type} = $3;
			} else {
				my @span_nodes = $resultTable->findnodes('//div[@class="fullResults fullResultsEnd"]/div[@class="dogHeadText"]/div/span');
				#print "Nodes: " . scalar(@span_nodes) . "\n";
				my $grade = $span_nodes[0]->findvalue('.');
				my $distance = $span_nodes[2]->findvalue('./span');
				if( $distance =~ m/^\s*(\d+)m\s*$/i ) { $distance = $1; }
				
				$dayraces->{$dogHeadTextH5}->{grade} = $grade;
				$dayraces->{$dogHeadTextH5}->{distance} = $distance;
				$dayraces->{$dogHeadTextH5}->{race_type} = 'Flat';
			}
			my $resultRaceDog = $resultTable->findnodes('./table[@class="grid resultRaceDog"]')->[0];
			next if( ! $resultRaceDog );
			
			my $title_row = $resultRaceDog->findnodes('./tr[@class="noSpace"]')->[0];
			# Store the order of the columns in the grid table
			my @th_columns = $title_row->findvalues('./th');
			
			# Get all rows in the table
			# Rows 3, 6, 9, 12, 15 and 18 are the rows that have the data of the classifications (1 - 6)
			my @table_rows_temp = $resultRaceDog->findnodes('./tr');
			
			# Remove rows of class "cp" which are not necessary
			my @table_rows = ();
			foreach my $row (@table_rows_temp) {
				my $class = $row->findvalue('./@class');
				my @childs = $row->findnodes('./td');
				next if( $class =~ m/^\s*cp\s*$/ );
				
				# Ignore rows that have not the same number of columns as defined in the table header columns
				next if( scalar(@childs) < scalar(@th_columns) );
				push(@table_rows, $row);
			}
			
			$dayraces->{$dogHeadTextH5}->{runners} = [];
			
			# Now I only have the rows from each dog (and track)
			foreach my $row (@table_rows) {
				my $race_dog = ();				
				
				my @columns = $row->findnodes('./td');
				my $idx = 0;
				foreach my $column ( @columns ) {					
					my $class = $column->findvalue('./@class');
					if( $class eq 'dog' ) {
						my $href = $column->findvalue('./a/@href');
						my $trap = $column->findvalue('./a/@class');
						$race_dog->{info_url} = $href;
						$race_dog->{trap} = $trap;
						# http://www.racingpost.com/greyhounds/dog_home.sd?dog_id=371838',
						if( $href =~ m/dog_home\.sd\?dog_id\=(\d+)\s*$/ ) {
							$race_dog->{dog_id} = $1;
						}
					}
					
					my $value = $column->findvalue('.');
					$race_dog->{lc($th_columns[$idx++])} = $value;
					
					# For now delete this key because it has some "strange" characters
					delete($race_dog->{dist});
				}
				
				my $dog_values = ();
				if( exists($dogs->{$race_dog->{dog}}) ) {
					$dog_values = $dogs->{$race_dog->{dog}};
				}
				else {
					$dog_values = get_dog_info(get_request($race_dog->{info_url}), $race_dog->{dog});
					$dogs->{$race_dog->{dog}} = $dog_values;
				}
				
				foreach my $key ( keys %{$dog_values} ) {
					$race_dog->{$key} = $dog_values->{$key};
				}
				
				push(@{$dayraces->{$dogHeadTextH5}->{runners}}, $race_dog);
			}
		}		
		print $FH2 "OK\n";		
	};
	
	if( $@ ) {
		print $FH2 "FAILED\n";
		print "Exception: $@";
	}
		
	$tree->delete;
	
	close($FH2);
	return ($dogs, $dayraces);
}

sub parse_html_day_future_races_table {
	my ($dogs, $html_file) = @_;
	
	my $dayraces = ();
		
	open(my $FH2, ">>", "future_races_processed.txt") or exit;
	print $FH2 "Processing file [$html_file] .........  ";
	
	print "Processing file [$html_file] ...\n";
	my $tree= HTML::TreeBuilder::XPath->new;

	eval {		
		$tree->parse_file($html_file);
		print "Parsed file [$html_file] ...\n";
		my $tabBlock = $tree->findnodes('//div[@class="tabBlock"]')->[0];
		my $tabContent = $tabBlock->findnodes('//div[@class="tabContent tabCube"]')->[0];
		my $tableSingleMeeting = $tabContent->findnodes('//table[@class="singleMeeting"]')->[0];		
		
		my @tableRows = $tableSingleMeeting->findnodes('//tr');
		print "Got [" . scalar(@tableRows) . "] rows\n";
		my $row_idx = 0;
		my $rid = 1;
		my $date_time1 = undef;
		my $date_time2 = undef;
		foreach my $tableRow ( @tableRows ) {			
			if( $row_idx == 0 ) {
				print "GETTING INFO FROM THE RACE HEADER\n";
				# First row is the header row and has the information about each one of the races (1 or 2 races by group of six rows)
				my $header1 = $tableRow->findnodes('./th[@class="head"]')->[0];
				my $header2 = $tableRow->findnodes('./th[@class="head"]')->[1];
				
				my $header1a = $header1->findvalue('./div[@class="title"]/a[@class="title"]') if( $header1);
				my $header2a = $header2->findvalue('./div[@class="title"]/a[@class="title"]') if( $header2);
				
				# 14:08 - Race 1 (A8) 380m
				# 18:28 - London Road Stakes (A1) 400m
				if( $header1a and $header1a =~ m/^(\d+\:\d+)\s+\-\s+(.*)\((.*)\)\s+(\d+\w*)\s*/ ) {
					$date_time1 = $1;
					#$dayraces->{$race_id1}->{date_time} = $1;
					$dayraces->{$date_time1}->{grade} = $3;
					$dayraces->{$date_time1}->{distance} = $4;
					$dayraces->{$date_time1}->{race_type} = 'Flat';
				}
				
				if( $header2a and $header2a =~ m/^(\d+\:\d+)\s+\-\s+(.*)\((.*)\)\s+(\d+\w*)\s*/ ) {
					$date_time2 = $1;
					#$dayraces->{$race_id2}->{date_time} = $1;
					$dayraces->{$date_time2}->{grade} = $3;
					$dayraces->{$date_time2}->{distance} = $4;
					$dayraces->{$date_time2}->{race_type} = 'Flat';
				}
				
				$dayraces->{$date_time1}->{runners} = [];
				$dayraces->{$date_time2}->{runners} = [];
				
				$row_idx++;
				next;
			}
			elsif( $row_idx >= 1 and $row_idx <= 6 ) {
				my @tdchilds = $tableRow->findnodes('./td');
				my $number_tds = scalar(@tdchilds);
				for(my $i = 0; $i < ($number_tds / 2); $i++ ) {
					my $dog = ();
					$dog->{trap} = $tdchilds[($i*2)]->findvalue('./a/@class');
					$dog->{info_url} = $tdchilds[($i*2)]->findvalue('./a/@href');
					$dog->{dog} = $tdchilds[($i*2)]->findvalue('./a');
					if( $dog->{info_url} =~ m/dog_home\.sd\?dog_id\=(\d+)\s*$/ ) {
						$dog->{dog_id} = $1;
					}
					my @trainernodes = $tdchilds[(($i*2)+1)]->findnodes('./a');
					my $trainer = undef;
					if( scalar(@trainernodes) == 2 ) {
						$trainer = $tdchilds[(($i*2)+1)]->findnodes('./a')->[1];
					} elsif( scalar(@trainernodes) == 1 ) {
						$trainer = $tdchilds[(($i*2)+1)]->findnodes('./a')->[0];
					}
					$dog->{trainer} = $trainer->findvalue('.') if( $trainer );				
					
					my $dog_values = ();
					if( exists($dogs->{$dog->{dog}}) ) {
						$dog_values = $dogs->{$dog->{dog}};
					}
					else {
						$dog_values = get_dog_info(get_request($dog->{info_url}), $dog->{dog});
						$dogs->{$dog->{dog}} = $dog_values;
					}
					
					foreach my $key ( keys %{$dog_values} ) {
						$dog->{$key} = $dog_values->{$key};
					}
					
					if( $i == 0 ) {
						push(@{$dayraces->{$date_time1}->{runners}}, $dog);
					} else {
						push(@{$dayraces->{$date_time2}->{runners}}, $dog);
					}
				}
				$row_idx ++;				
			}
			elsif( $row_idx >6 ) {
				# Reinitialize counter and process next block
				$row_idx = 0;
				$date_time1 = undef;
				$date_time2 = undef;
			}		
		}
		#print Dumper($dayraces);
		
		print $FH2 "OK\n";		
	};
	
	if( $@ ) {
		print $FH2 "FAILED\n";
		print "Exception: $@";
	}
		
	$tree->delete;
	
	close($FH2);
	return ($dogs, $dayraces);
}


1;