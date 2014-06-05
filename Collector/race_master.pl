use strict;
use warnings;

use Data::Dumper;
use LWP::UserAgent;

my $stadium_ids = ();
my $wget_cmd = 'wget.exe';

sub usage() {
    print STDERR"\nUsage: $0 DATE_TO_PROCESS ('yyyy-mm-dd')\n\n";
}

my $date = $ARGV[0];
chomp($date);
if( ! $date or $date !~ m/^\d{4}\-\d{2}\-\d{2}$/ ) {
    print STDERR "Invalid date provided.\n";
    usage();
    exit -1;
}

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

#########################################################################################################################
# FOR RACES IN THE PAST
#########################################################################################################################
sub parse_results_response {
	my ($response, $date) = @_;
	
	my $stadiums = ();
	
	my $content = $response->content;
	
	# <tr><th title="Meeting" class="meeting"><a href="#18" title="BELLE VUE">
	while( $content =~ m/\<tr\>\<th\s+title\=\"Meeting\"\s+class\=\"meeting\"\>\<a\s+href\=\"\#(\d+)\"\s+title\=\"(\S+[^\"])\"\>/ig ) {
		$stadiums->{$2}->{$date}->{id} = $1;
	}
	
	return $stadiums;
}

sub get_results_urls {
	my ($stadiums) = @_;
	
	my @urls = ();
	
	# http://www.racingpost.com/greyhounds/result_by_meeting_full.sd?r_date=$DATE&meeting_id=MEETING_ID
	foreach my $stadium ( keys %{$stadiums} ) {
		foreach my $date ( keys %{$stadiums->{$stadium}} ) {
			my $results_url = 'http://www.racingpost.com/greyhounds/result_by_meeting_full.sd?r_date=' . $date . '&meeting_id=' . $stadiums->{$stadium}->{$date}->{id};
			$stadium_ids->{$stadium}->{id} = $stadiums->{$stadium}->{$date}->{id} if( not exists($stadium_ids->{$stadium}->{id}) );
			#print STDOUT $results_url . "\n";
			push( @urls, $results_url);
		}
	}
	
	return @urls;
}

sub get_day_races_results {
	my ($date) = @_;
	
	return if( !$date or $date !~ m/^\d{4}\-\d{2}\-\d{2}$/);
	
	# http://www.racingpost.com/greyhounds/result_head.sd?r_date=2013-11-07
	my $line = 'http://www.racingpost.com/greyhounds/result_head.sd?r_date=' . $date;
	
	my $day_directory = 'C:\\Temp\\CURRENT_RACES\\' . $date;
	mkdir("C:\\Temp") if( ! -d "C:\\Temp" );
	mkdir("C:\\Temp\\CURRENT_RACES") if( ! -d "C:\\Temp\\CURRENT_RACES" );
	mkdir($day_directory) if( ! -d $day_directory );
	
	unless( -e $day_directory or mkdir $day_directory ) {
		die "Unable to create $day_directory\n";
	}
	
	my $response = get_request($line);
	if( ! defined $response )
	{
		print STDOUT "URL $line failed.\n";
		# TODO: store the failure in a file to retry later
	}
	else {
		# Get all the Stadiums IDs that had race events in a particular date
		my $stadiums = parse_results_response($response, $date);
				
		# Get all the URLs that point to Result Tables - these tables will be retrieved later
		my @results = get_results_urls($stadiums);
		
		open(my $FH1, ">>", "races_results_urls.txt") or next;
		for my $url (@results) {
			print $FH1 "$url\n";
			my $out_file = '';
			
			if( $url =~ m/^\s*http\:\/\/www\.racingpost\.com\/greyhounds\/result_by_meeting_full\.sd\?(.*)$/i ) {
				$out_file = $day_directory . '\\' . $1 . '.html';
				$out_file =~ s/\&/_/;
			}
			if( $out_file ) {
				`$wget_cmd -O $out_file "$url"`;
			}
		}
		close($FH1);
	}
}

get_day_races_results($date);

use Cwd;
my $currdir = getcwd;

my $daydirectory = 'C:\\Temp\\CURRENT_RACES\\' . $date;
system("perl parse_races_files.pl --operation PARSEFILES --dir \"$daydirectory\" --racesstore racesstore.store --dogsstore dogsstore.store");

chdir($daydirectory);

system("perl \"$currdir\\parse_races_files.pl\" --operation PROCESSDATA --racesstore racesstore.store --dogsstore dogsstore.store");
print "\n\n\nCURRENT DIR: $currdir\n\n\n";

chdir($currdir);

1;


