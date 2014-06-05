package Dogs::DBConnector;

use strict;
use warnings;

use DBIx::Simple;
use DBIx::ProcedureCall qw(F_ADVANCED_PREDICTION);
use Data::Dumper;

my $STADIUMS_BY_ID = {
    '1' => 'CRAYFORD',
	'4' => 'MONMORE',
    '5' => 'HOVE',
    '6' => 'NEWCASTLE',
    '7' => 'OXFORD',
    '9' => 'WIMBLEDON',
    '11' => 'ROMFORD',
    '13' => 'HENLOW',
	'16' => 'YARMOUTH',
	'17' => 'HALL_GREEN',
	'18' => 'BELLE_VUE',
	'21' => 'SHELBOURNE_PARK',
	'25' => 'PETERBOROUGH',
	'33' => 'NOTTINGHAM',
    '34' => 'SHEFFIELD',
	'35' => 'POOLE',
    '38' => 'SHAWFIELD',
    '39' => 'SWINDON',
	'41' => 'CLONMEL',
    '42' => 'CORK',
	'43' => 'HAROLDS_CROSS',
	'45' => 'DUNDALK',
	'57' => 'TRALEE',
    '61' => 'SUNDERLAND',
	'62' => 'PERRY_BAR',
	'63' => 'MILDENHALL',
    '66' => 'DONCASTER',    
	'69' => 'HARLOW',    
    '70' => 'SITTINGBOURNE',
    '76' => 'KINSLEY',
    '83' => 'COVENTRY',			
	'86' => 'PELLAW_GRANGE'
};

my $STADIUMS_BY_NAME = {
    'CRAYFORD' => '1',
	'MONMORE' => '4',
    'HOVE' => '5',
    'NEWCASTLE' => '6',
    'OXFORD' => '7',
    'WIMBLEDON' => '9',
    'ROMFORD' => '11',
    'HENLOW' => '13',
	'YARMOUTH' => '16',
	'HALL_GREEN' => '17',
	'BELLE_VUE' => '18',
	'SHELBOURNE_PARK' => '21',
	'PETERBOROUGH' => '25',
	'NOTTINGHAM' => '33',
    'SHEFFIELD' => '34',
	'POOLE' => '35',
    'SHAWFIELD' => '38',
    'SWINDON' => '39',
	'CLONMEL' => '41',
    'CORK' => '42',
	'HAROLDS_CROSS' => '43',
	'DUNDALK' => '45',    
	'TRALEE' => '57',
    'SUNDERLAND' => '61',
    'PERRY_BAR' => '62',
    'MILDENHALL' => '63',
	'DONCASTER' => '66',
	'HARLOW' => '69',    
    'SITTINGBOURNE' => '70',            
    'KINSLEY' => '76',    
    'COVENTRY' => '83',
	'PELLAW_GRANGE' => '86'
};

my $idx_to_months = {
	'01' => 'JAN', 
	'02' => 'FEB', 
	'03' => 'MAR',
	'04' => 'APR',
	'05' => 'MAY',
	'06' => 'JUN',
	'07' => 'JUL',
	'08' => 'AUG',
	'09' => 'SEP', 
	'10' => 'OCT', 
	'11' => 'NOV',
	'12' => 'DEC'
};

sub quote_chars {
	my ($str) = @_;
	
	$str =~ s/'/''/;
	
	return $str;
}

sub executeDBQuery {
	my ($db, $query) = @_;
	
	my $res = $db->query($query) if( $db and $query and $query ne '' );
	
	return $res;
}

sub process_past_races_data {
	my ($races) = @_;	
	
	my $DB_LOG_FILE = 'racesstore_DATABASE_QUERIES.log';	
	open(my $LOGFH, ">", $DB_LOG_FILE) or die "Unable to open file $DB_LOG_FILE";
	my $h = select($$LOGFH);
	$|=1;
	select($h);
	
	my %dbdata = (
		dsn			=> 'dbi:Oracle:database=test;HOST=192.168.1.1;PORT=1521;SID=test',
		username	=> 'dbuser',
		password	=> 'dbpassword',        
    );
		
	my $db = DBIx::Simple->connect( $dbdata{dsn}, $dbdata{username}, $dbdata{password}, { RaiseError => 1, AutoCommit => 0} );
	
	eval {
	
		# STADIUMS
		my $existent_stadiums = $db
		->query("select std_id, name, num_tracks, city, country from stadiums")
		-> map_hashes('std_id');
				
		#print Dumper($existent_stadiums);
				
		foreach my $stadium ( keys %{$races} ) {
			my $STD_ID = $stadium;
			$STD_ID = $STADIUMS_BY_NAME->{$stadium} if( exists($STADIUMS_BY_NAME->{$stadium}) );
			
			my $STD_NAME = $stadium;
			$STD_NAME = quote_chars($STADIUMS_BY_ID->{$stadium}) if( exists($STADIUMS_BY_ID->{$stadium}) );
			
			#Skip already existent rows
			next if( exists($existent_stadiums->{$STD_ID}) );
			
			my $NUM_TRACKS = 6;			
			my $CITY = quote_chars($STD_NAME);
			my $COUNTRY = 'United Kingdom';
			
			print "\nAdding Stadium [$STD_NAME] \t with ID [$STD_ID] .........";
			
			my $query = "insert into stadiums (std_id, name, num_tracks, city, country) values ('$STD_ID', '$STD_NAME', '$NUM_TRACKS', '$CITY', '$COUNTRY')"; 
			executeDBQuery($db, $query);
			
			print $LOGFH "$query;\n";
			
			print "OK\n";
		}
		$db->commit;
		
		# RACES
		# GET EXISTENT RACES FIRST
		my $exist_races = $db
						->query("select concat(date_time, stadiums_std_id) as mykey, stadiums_std_id, date_time, race_id from races")
						-> map_hashes('mykey');						
		
		# NEXT GET EXISTENT RACES_DETAILS
		my $exist_race_details = $db
						->query("select concat(races_race_id, greyhounds_grey_id) as mykey, greyhounds_grey_id, position from races_details")
						-> map_hashes('mykey');
		
		foreach my $stadium ( keys %{$races} ) {
			my $STD_ID = $stadium;
			$STD_ID = $STADIUMS_BY_NAME->{$stadium} if( exists($STADIUMS_BY_NAME->{$stadium}) );			
			foreach my $date ( keys %{$races->{$stadium}} ) {
				my $RACE_NUM = 1;
				foreach my $time ( sort keys %{$races->{$stadium}->{$date}} ) {
					my ($hour,$min) = split(/[-:]+/, $time);
					my $am_pm = (($hour < 10 or $hour >= 12 ) ? 'PM' : 'AM');
					# DB ==> '29-SEP-13 08.19.00.000000 PM'
					# MEU => '2014-02-07 11.03.00.000000 PM'
					my $conv_date = $date;
					if( $date =~ m/^\s*\d\d(\d\d)\-(\d+)\-(\d+)\s*$/ ) {
						$conv_date = $3 . '-' . $idx_to_months->{$2} . '-' . $1;
					}
					my $key_hour = ($hour < 10 ? '0' . $hour : $hour);
					my $row_key = "$conv_date $key_hour.$min.00.000000 $am_pm" . $STD_ID;
					print "TIME KEY: $row_key\n";
					
					# Time of races is in 12 hour format
					$hour += 12 if( $hour < 11 );
					my $TIMESTAMP = "$date $hour:$min:00";

					my $GRADE = quote_chars($races->{$stadium}->{$date}->{$time}->{grade});
					$GRADE = 'NG' if( not $GRADE or $GRADE eq '' );
					
					my $DISTANCE = $races->{$stadium}->{$date}->{$time}->{distance};
					if( $DISTANCE =~ m/^\s*(\d+)(\w+)\s*$/ ) {
						$DISTANCE = $1;
						if( $2 eq 'y' ) {
							$DISTANCE = int($DISTANCE * 0.9144);
						}
					}					
					$DISTANCE = 999 if( not $DISTANCE or $DISTANCE eq '' );
					$DISTANCE = ($DISTANCE * 10) if( $DISTANCE < 100 );
					
					my $RACE_TYPE = quote_chars($races->{$stadium}->{$date}->{$time}->{race_type});					
					
					my $RACE_ID = undef;
					my $mode = 'INSERT';					
					if( exists($exist_races->{"$row_key"}) and exists($exist_races->{"$row_key"}->{stadiums_std_id}) and $exist_races->{"$row_key"}->{stadiums_std_id} eq $STD_ID ) {
						$RACE_ID = $exist_races->{"$row_key"}->{race_id};
						$mode = 'UPDATE';						
					}
					else {
						$RACE_ID = $db->query("select races_seq.nextval from dual")->text;
						$RACE_ID =~ s/'//g;
						chomp($RACE_ID);					
					}
					print "MODE: $mode\n";
					
					# RACES
					# FIRST ADD RACE IF IT DOESN?T EXIST YET					
					if( $mode ne 'UPDATE' or not exists($exist_races->{"$row_key"}) or $exist_races->{"$row_key"}->{race_id} ne $RACE_ID ) {
						print "\nAdding race\t $RACE_ID \tthat took place at [$date] and [$time] \tin the $stadium \tStadium  .........";
						
						my $query ="insert into races (race_id, stadiums_std_id, date_time, race_number, track_length, grade, race_type)"
								. " values ('$RACE_ID', '$STD_ID', TO_DATE('$TIMESTAMP','yyyy/mm/dd hh24:mi:ss'), '$RACE_NUM', '$DISTANCE', '$GRADE', '$RACE_TYPE')";
						executeDBQuery($db, $query);
						$exist_races->{"$row_key"}->{race_id} = $RACE_ID;
						print $LOGFH "$query;\n";
						
						print "OK\n\n";
					}					
					else {
						print "\n*********  Skipping Race [$RACE_ID] \t..........  ALREADY EXISTS\n\n";
					}
					
					my $exist_dog_id = $db
						->query("select grey_id, name from GREYHOUNDS")
						-> map_hashes('grey_id');
						
					for my $runner ( @{$races->{$stadium}->{$date}->{$time}->{runners}} ) {
						my $GREY_ID = $runner->{dog_id};						
						my $GREY_NAME = quote_chars($runner->{dog});
						my $TRAINER = quote_chars($runner->{trainer});
						$TRAINER = 'NO_TRAINER' if( not $TRAINER or $TRAINER eq "" );
						my $BIRTH_DATE = $runner->{birth_date};
						# Unknown birth date
						$BIRTH_DATE = '01-01-1970' if( not $BIRTH_DATE );
						
						# GREYHOUNDS
						# NEXT ADD GREYHOUND IF IT DOES NOT EXIST YET
						if( not exists($exist_dog_id->{$GREY_ID}) ) {
							print "Adding greyhound  [$GREY_NAME] \t.........";
							
							my $query ="insert into Greyhounds (grey_id, name, trainer, birth_date)"
								. " values ('$GREY_ID', '$GREY_NAME', '$TRAINER', TO_DATE('$BIRTH_DATE','dd/mm/yyyy'))";
							executeDBQuery($db, $query);
							$exist_dog_id->{$GREY_ID}->{name} = $GREY_NAME;
							print $LOGFH "$query;\n";
							
							print "OK\n";
						}
						else {
							print "\n*********  Skipping Greyhound [$GREY_NAME] \t..........  ALREADY EXISTS\n\n";
						}
			
						$RACE_ID =~ s/'//g;
						chomp($RACE_ID);

						
						# RACES_RESULTS
						# FINALLY ADD THE RACE RESULT DETAIL
						my $TRACK_NUMBER = '99';
						if( $runner->{trap} =~ m/trap_(\d+)/ ) {
							$TRACK_NUMBER = $1;
						}
						my $RACE_COMPLETED = (exists($runner->{time}) ? 1 : 0);
						
						my $RACE_POSITION = '99';						
						if( $runner->{position} =~ m/\s*(\d+)(st|nd|rd|th)\s*$/ ) {
							$RACE_POSITION = $1;
						}
						
						my $RACE_TIME = '';
						if( $runner->{time} =~ m/\s*(\d+\.\d+)\s*$/ ) {
							$RACE_TIME = $1; #$RACE_TIME =~ s/\./,/;
						}
						
						if( not exists($exist_race_details->{$RACE_ID . $GREY_ID}) ) {
							print "Adding RACE_DETAIL for race [$RACE_ID] and Greyhound  [$GREY_NAME] \t.........";
							
							my $query ="insert into RACES_DETAILS (Races_race_id, Greyhounds_grey_id, track_number, race_completed, position, time)"
								. " values ('$RACE_ID', '$GREY_ID', '$TRACK_NUMBER', '$RACE_COMPLETED', '$RACE_POSITION', '$RACE_TIME')";
							executeDBQuery($db, $query);
							$exist_race_details->{$RACE_ID . $GREY_ID} = 1;
							print $LOGFH "$query;\n";
							
							print "OK\n";
						}
						elsif($mode eq 'UPDATE') {
							if( $exist_race_details->{$RACE_ID . $GREY_ID}->{position} eq '' or $exist_race_details->{$RACE_ID . $GREY_ID}->{position} > 0 ) {
								print "Skipping update of RACE_DETAIL for race [$RACE_ID] and Greyhound  [$GREY_NAME] because his score was already updated fro this race..";
								next;
							}
							print "Updating RACE_DETAIL for race [$RACE_ID] and Greyhound  [$GREY_NAME] \t.........";
							
							my $query ="update RACES_DETAILS set race_completed='$RACE_COMPLETED', position='$RACE_POSITION', time='$RACE_TIME' where"
								. " Races_race_id='$RACE_ID' and Greyhounds_grey_id='$GREY_ID'";
							executeDBQuery($db, $query);
							print $LOGFH "$query;\n";
							
							print "OK\n";
						}
						else {
							print "\n*********  Skipping RACE_DETAIL for race [$RACE_ID] and Greyhound  [$GREY_NAME] \t..........  ALREADY EXISTS\n\n";
						}
					}
					$RACE_NUM++;
				}
			}
			$db->commit;
		}		
	};
	
	close($LOGFH);
	
	if( $@ ) {
		$db->rollback;
		print "Exception: $@";		
	}	
	
	$db->disconnect;
}

sub process_future_races_data {
	my ($races) = @_;	
	
	my $DB_LOG_FILE = 'racesstore_DATABASE_QUERIES.log';	
	open(my $LOGFH, ">", $DB_LOG_FILE) or die "Unable to open file $DB_LOG_FILE";
	my $h = select($$LOGFH);
	$|=1;
	select($h);
	
	my %dbdata = (
		dsn			=> 'dbi:Oracle:database=test;HOST=192.168.1.1;PORT=1521;SID=test',
		username	=> 'dbuser',
		password	=> 'dbpassword',        
    );
		
	my $db = DBIx::Simple->connect( $dbdata{dsn}, $dbdata{username}, $dbdata{password}, { RaiseError => 1, AutoCommit => 0} );
	
	eval {
	
		# STADIUMS
		my $existent_stadiums = $db
		->query("select std_id, name, num_tracks, city, country from stadiums")
		-> map_hashes('std_id');
				
		#print Dumper($existent_stadiums);
				
		foreach my $stadium ( keys %{$races} ) {
			my $STD_ID = $stadium;
			$STD_ID = $STADIUMS_BY_NAME->{$stadium} if( exists($STADIUMS_BY_NAME->{$stadium}) );
			
			my $STD_NAME = $stadium;
			$STD_NAME = quote_chars($STADIUMS_BY_ID->{$stadium}) if( exists($STADIUMS_BY_ID->{$stadium}) );
			
			#Skip already existent rows
			next if( exists($existent_stadiums->{$STD_ID}) );
			
			my $NUM_TRACKS = 6;			
			my $CITY = quote_chars($STD_NAME);
			my $COUNTRY = 'United Kingdom';
			
			print "\nAdding Stadium [$STD_NAME] \t with ID [$STD_ID] .........";
			
			my $query = "insert into stadiums (std_id, name, num_tracks, city, country) values ('$STD_ID', '$STD_NAME', '$NUM_TRACKS', '$CITY', '$COUNTRY')"; 
			executeDBQuery($db, $query);
			
			print $LOGFH "$query;\n";
			
			print "OK\n";
		}
		$db->commit;
		
		# RACES
		# GET EXISTENT RACES FIRST
		my $exist_races = $db
						->query("select concat(date_time, stadiums_std_id) as mykey, stadiums_std_id, date_time, race_id from Races")
						-> map_hashes('mykey');
						
		# NEXT GET EXISTENT RACES_DETAILS
		my $exist_race_details = $db
						->query("select concat(races_race_id, greyhounds_grey_id) as mykey, greyhounds_grey_id, position from races_details")
						-> map_hashes('mykey');
						
		foreach my $stadium ( keys %{$races} ) {
			my $STD_ID = $stadium;
			$STD_ID = $STADIUMS_BY_NAME->{$stadium} if( exists($STADIUMS_BY_NAME->{$stadium}) );			
			foreach my $date ( keys %{$races->{$stadium}} ) {
				my $RACE_NUM = 1;
				foreach my $time ( sort keys %{$races->{$stadium}->{$date}} ) {
					my ($hour,$min) = split(/[-:]+/, $time);
					
					next if( not $hour or $hour eq "" or not $min or $min eq "");
					my $am_pm = (($hour < 10 or $hour >= 12 ) ? 'PM' : 'AM');
					# DB ==> '29-SEP-13 08.19.00.000000 PM'
					# MEU => '2014-02-07 11.03.00.000000 PM'
					my $conv_date = $date;
					if( $date =~ m/^\s*\d\d(\d\d)\-(\d+)\-(\d+)\s*$/ ) {
						$conv_date = $3 . '-' . $idx_to_months->{$2} . '-' . $1;
					}
					my $key_hour = ($hour < 10 ? '0' . $hour : $hour);
					my $row_key = "$conv_date $key_hour.$min.00.000000 $am_pm" . $STD_ID;
					print "TIME KEY: $row_key\n";
					
					# Time of races is in 12 hour format
					$hour += 12 if( $hour < 11 );
					my $TIMESTAMP = "$date $hour:$min:00";

					my $GRADE = quote_chars($races->{$stadium}->{$date}->{$time}->{grade});
					$GRADE = 'NG' if( not $GRADE or $GRADE eq '' );
					
					my $DISTANCE = $races->{$stadium}->{$date}->{$time}->{distance};
					if( $DISTANCE =~ m/^\s*(\d+)(\w+)\s*$/ ) {
						$DISTANCE = $1;
						if( $2 eq 'y' ) {
							$DISTANCE = int($DISTANCE * 0.9144);
						}
					}					
					$DISTANCE = 999 if( not $DISTANCE or $DISTANCE eq '' );
					$DISTANCE = ($DISTANCE * 10) if( $DISTANCE < 100 );
					
					my $RACE_TYPE = quote_chars($races->{$stadium}->{$date}->{$time}->{race_type});					
					
					my $RACE_ID = undef;
					my $mode = 'INSERT';					
					if( exists($exist_races->{"$row_key"}) and exists($exist_races->{"$row_key"}->{stadiums_std_id}) and $exist_races->{"$row_key"}->{stadiums_std_id} eq $STD_ID ) {
						$RACE_ID = $exist_races->{"$row_key"}->{race_id};
						$mode = 'UPDATE';
					}
					else {
						$RACE_ID = $db->query("select races_seq.nextval from dual")->text;
						$RACE_ID =~ s/'//g;
						chomp($RACE_ID);					
					}
					
					# RACES
					# FIRST ADD RACE IF IT DOESN?T EXIST YET
					if( $mode ne 'UPDATE' or not exists($exist_races->{"$row_key"}) or $exist_races->{"$row_key"}->{race_id} ne $RACE_ID ) {
						print "\nAdding race\t $RACE_ID \tthat will take place at [$date] and [$time] \tin the $stadium \tStadium  .........";
						
						my $query ="insert into races (race_id, stadiums_std_id, date_time, race_number, track_length, grade, race_type)"
								. " values ('$RACE_ID', '$STD_ID', TO_DATE('$TIMESTAMP','yyyy/mm/dd hh24:mi:ss'), '$RACE_NUM', '$DISTANCE', '$GRADE', '$RACE_TYPE')";
						executeDBQuery($db, $query);
						$exist_races->{"$row_key"}->{race_id} = $RACE_ID;
						print $LOGFH "$query;\n";
						
						print "OK\n\n";
					}
					else {
						print "\n*********  Skipping Race [$RACE_ID] \t..........  ALREADY EXISTS\n\n";
					}
					
					my $exist_dog_id = $db
						->query("select grey_id, name from GREYHOUNDS")
						-> map_hashes('grey_id');
						
					for my $runner ( @{$races->{$stadium}->{$date}->{$time}->{runners}} ) {
						my $GREY_ID = $runner->{dog_id};
						my $GREY_NAME = quote_chars($runner->{dog});
						my $TRAINER = quote_chars($runner->{trainer});
						$TRAINER = 'NO_TRAINER' if( not $TRAINER or $TRAINER eq "" );
						my $BIRTH_DATE = $runner->{birth_date};
						# Unknown birth date
						$BIRTH_DATE = '01-01-1970' if( not $BIRTH_DATE );
						
						# GREYHOUNDS
						# NEXT ADD GREYHOUND IF IT DOES NOT EXIST YET
						if( $GREY_ID and not exists($exist_dog_id->{$GREY_ID}) ) {
							print "Adding greyhound  [$GREY_NAME] \t.........";
							
							my $query ="insert into Greyhounds (grey_id, name, trainer, birth_date)"
								. " values ('$GREY_ID', '$GREY_NAME', '$TRAINER', TO_DATE('$BIRTH_DATE','dd/mm/yyyy'))";
							executeDBQuery($db, $query);
							$exist_dog_id->{$GREY_ID}->{name} = $GREY_NAME;
							print $LOGFH "$query;\n";
							
							print "OK\n";
						}
						else {
							print "\n*********  Skipping Greyhound [$GREY_NAME] \t..........  ALREADY EXISTS\n\n";
						}
			
						$RACE_ID =~ s/'//g;
						chomp($RACE_ID);
						
						# RACES_RESULTS
						# FINALLY ADD THE RACE RESULT DETAIL
						my $TRACK_NUMBER = '99';
						if( $runner->{trap} =~ m/trap_(\d+)/ ) {
							$TRACK_NUMBER = $1;
						}
						my $RACE_COMPLETED = (exists($runner->{time}) ? 1 : 0);
						
						my $RACE_POSITION = '';						
						if( $runner->{position} and $runner->{position} =~ m/\s*(\d+)(st|nd|rd|th)\s*$/ ) {
							$RACE_POSITION = $1;
						}
						
						my $RACE_TIME = '';
						if( $runner->{time} and $runner->{time} =~ m/\s*(\d+\.\d+)\s*$/ ) {
							$RACE_TIME = $1; #$RACE_TIME =~ s/\./,/;
						} 
						
						if( $RACE_ID and $GREY_ID and not exists($exist_race_details->{$RACE_ID . $GREY_ID}) ) {
							print "Adding RACE_DETAIL for race [$RACE_ID] and Greyhound  [$GREY_NAME] \t.........";
							
							my $query ="insert into RACES_DETAILS (Races_race_id, Greyhounds_grey_id, track_number, race_completed, position, time)"
								. " values ('$RACE_ID', '$GREY_ID', '$TRACK_NUMBER', '$RACE_COMPLETED', '$RACE_POSITION', '$RACE_TIME')";
							executeDBQuery($db, $query);
							$exist_race_details->{$RACE_ID . $GREY_ID} = 1;
							print $LOGFH "$query;\n";
							
							print "OK\n";
						}
						elsif($mode eq 'UPDATE') {
						    if( $exist_race_details->{$RACE_ID . $GREY_ID}->{position} eq '' or $exist_race_details->{$RACE_ID . $GREY_ID}->{position} > 0 ) {
								print "Skipping update of RACE_DETAIL for race [$RACE_ID] and Greyhound  [$GREY_NAME] because his score was already updated fro this race..";
								next;
							}
							
							print "Updating RACE_DETAIL for race [$RACE_ID] and Greyhound  [$GREY_NAME] \t.........";
							
							my $query ="update RACES_DETAILS set race_completed='$RACE_COMPLETED', position='$RACE_POSITION', time='$RACE_TIME' where"
								. " Races_race_id='$RACE_ID' and Greyhounds_grey_id='$GREY_ID'";
							executeDBQuery($db, $query);
							print $LOGFH "$query;\n";
							
							print "OK\n";
						}
						else {
							print "\n*********  Skipping RACE_DETAIL for race [$RACE_ID] and Greyhound  [$GREY_NAME] \t..........  ALREADY EXISTS\n\n";
						}

						#my $runner_odds_preds = ();
						# Add advanced predictions for new races
						if($GREY_ID and $RACE_ID) {
							my $adv_prediction = undef;
							eval {								
								$adv_prediction = F_ADVANCED_PREDICTION($db->{dbh}, $RACE_ID, $GREY_ID);
								print "ADVANCED PREDICTION: $adv_prediction\n";
								
								if( $adv_prediction ) {
									my $query ="update predictions set prediction_advanced=$adv_prediction where races_details_race_id='$RACE_ID' and races_details_grey_id='$GREY_ID'";
									executeDBQuery($db, $query);
									print $LOGFH "$query;\n";
								}
							};
							
							print $@ if($@);
							
							# TODO: GET ODDS FROM BETFAIR FOR EACH RACE
							my $odd = 'N/A';
							my $odd_value = 1;
							if( exists($runner->{'sp'}) and $runner->{'sp'} ne '' ) {
								$odd = $runner->{'sp'};
							} else {
							    my $factor = 0;
								if( $adv_prediction and $adv_prediction > 0 ) {
									$factor = $adv_prediction % 10;
								}
								
								my $num1 = int(rand(10)) - $factor;
								my $num2 = int(rand(10));
								$num1 =1 if( $num1 < 1 );
								$num2 =1 if( $num2 < 1 );
								
								$odd = "$num1/$num2";
								$odd_value = $num1 / $num2;
							}
							$odd = 'N/A' if( not defined($odd) );
							print "RACE: [$RACE_ID] RUNNER: [$runner->{dog_id}]    ODD: [$odd]     ODDVALUE: [$odd_value]\n";
								
							eval {
								my $query ="insert into odds (races_details_race_id, races_details_grey_id, odd, odd_value) values ('$RACE_ID', '$GREY_ID', '$odd', '$odd_value')";
								executeDBQuery($db, $query);
								print $LOGFH "$query;\n";
							};
							
							if($@) {
								eval {
									my $query ="update odds set odd='$odd', odd_value='$odd_value' where races_details_race_id='$RACE_ID' and races_details_grey_id='$GREY_ID'";
									executeDBQuery($db, $query);
									print $LOGFH "$query;\n";
								};
								
								print $@ if($@);
							}
						}
					}
					
					$RACE_NUM++;
				}
			}
			$db->commit;
		}		
	};
	
	close($LOGFH);
	
	if( $@ ) {
		$db->rollback;
		print "Exception: $@";		
	}	
	
	$db->disconnect;
}

1;

