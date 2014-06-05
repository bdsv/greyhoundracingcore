package Dogs;

use strict;
use warnings;

use Data::Dumper;

use Cwd;
use Storable qw(lock_nstore lock_retrieve);
use File::Copy;
use Getopt::Long;
use Time::Local;
use Win32API::File qw(:Func :HANDLE_FLAG_);
use Fcntl ':flock';

use Dogs::DBConnector;
use Dogs::HTMLParser;
use Dogs::HTMLGetter;

select(STDOUT); # default
$| = 1;

local $Storable::canonical = 1;

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

my $tempPath = $ENV{'TEMP'};
my $lock_file = $tempPath . "\\lock_file.lock";

my $dogs_store = 'dogs_info.store';
my $races_store = 'races_info.store';

my $file_or_dir = undef;

my $DIRMODE = 0;
my $OPERATION = undef;

sub lock_process {
	my $LCK = undef;
	
	print STDOUT "\nLocking file .........";
	
	open($LCK, ">$lock_file") or die "Failed to lock file $lock_file, error: $^E";
	flock($LCK, LOCK_EX) or die "Failed to lock file $lock_file";
	
	print STDOUT "DONE\n";
	
	return $LCK;
}

sub unlock_process {
	my ($lck) = @_;
	
	print STDOUT "Unlocking file .........";
	
	close($lck) if( $lck );
	$lck = undef;
	
	print STDOUT "DONE\n";
}
	
sub usage() {
    print STDERR "\nUsage: $0
	\t\t\t--operation ( PARSEFILES | PROCESSDATA | DUMPDATA )
	\t\t\t--file FILE   -   HTML file to parse (used only with PARSEFILES operation)
	\t\t\t--dir or --path DIR   -   Directory which contains HTML files to parse   (used only with PARSEFILES operation)
	\t\t\t--racesstore RACES_STORE_FILE    -  Storable Hash file which contains or will contain races data
	\t\t\t--dogsstore DOGS_STORE_FILE   -  Storable Hash file which contains or will contain dogs data\n\n";
	
	exit -1;
}

GetOptions ("operation=s"	=> \$OPERATION,     # string
            "file|dir|path:s"	=> \$file_or_dir,   # string
			"racesstore:s"	=> \$races_store,   # string
            "dogsstore:s"	=> \$dogs_store)    # string
or usage();

if( not $OPERATION ) {
	print STDERR "Invalid Operation\n";
	usage();
}

$OPERATION = uc($OPERATION);

if( $OPERATION ne 'PROCESSFUTUREDATA' and $OPERATION ne 'PARSEFUTUREFILES' and $OPERATION ne 'PROCESSDATA' and $OPERATION ne 'DUMPDATA' and $OPERATION ne 'PARSEFILES' ) {
	print STDERR "Invalid Operation\n";
	usage();
}
	
if( $OPERATION eq 'PARSEFILES' or $OPERATION eq 'PARSEFUTUREFILES' ) {	
	if( ! $file_or_dir or $file_or_dir eq "" or ( not -f $file_or_dir and not -d $file_or_dir ) ) {
		print STDERR "Invalid File or Directory\n";
		usage();
	}	
}

sub load_data {
	print STDOUT "Loading data from the store ......... ";

	my $races = ();
	my $dogs = ();
	
	$dogs = lock_retrieve($dogs_store) if( -f $dogs_store );
	$races = lock_retrieve($races_store) if( -f $races_store );
	
	print STDOUT "DONE\n";
	
	return ($races, $dogs);
}

sub store_data {
	my ($races, $dogs) = @_;
	
	print STDOUT "\nStoring data to the store ......... ";
	
	my $lck = lock_process();
		
	eval {
		my $dogs_stored = lock_retrieve($dogs_store) if( -f $dogs_store );
		if( $dogs_stored ) {
			foreach my $key (keys %{$dogs_stored}) {
				$dogs->{$key} = $dogs_stored->{$key} if( not exists($dogs->{$key}) );
			}
		}
		
		# Create backup file
		copy($races_store, $races_store . ".bck");
		copy($dogs_store, $dogs_store . ".bck");
	
		# Store data on system		
		lock_nstore $races, $races_store;
		lock_nstore $dogs, $dogs_store;
	};
	
	print "Exception: $@" if ($@);
	
	unlock_process($lck);
	
	print STDOUT "DONE\n";
}
	
sub process_file {
	my ($races, $dogs, $file) = @_;
	
	print "\nPROCESSING FILE $file ...\n";
	eval {
		my $content = undef;
		{
			local $/ = undef;
			open FILE, $file or die "Couldn't open file $file: $!";
			$content = <FILE>;
			close FILE;
			my $stadium = undef;
			my $date = undef;
		
			# r_date=2010-01-01_meeting_id=4.html
			if( $file =~ m/r_date\=(\d+\-\d+\-\d+)_meeting_id\=(\d+)\.html/ ) {
				$date = $1;
				$stadium = "STADIUM_" . $2;
				$stadium = 	$STADIUMS_BY_ID->{$2} if( exists($STADIUMS_BY_ID->{$2}) );
			}
			else {
				print "Invalid file name: $file";
				return;
			}
	  
			unless( exists($races->{$stadium}) && exists($races->{$stadium}->{$date}) ) {
				print "Getting info for race in $date at the $stadium stadium ...\n";
				my $dayraces = undef;
				($dogs, $dayraces) = Dogs::HTMLParser::parse_html_day_races_results_table($dogs, $file);  
				$races->{$stadium}->{$date} = $dayraces;
			}
			
			store_data($races, $dogs);
		}
	};
	
	if($@) {
		print "Exception: $@";
	}
	
	print "PROCESSING FILE $file DONE\n\n";
}
	
sub process_future_file {
	my ($races, $dogs, $file) = @_;
	
	print "\nPROCESSING FILE $file ...\n";
	
	eval {
		my $content = undef;
		{
			local $/ = undef;
			open FILE, $file or die "Couldn't open file $file: $!";
			$content = <FILE>;
			close FILE;
			my $stadium = undef;
			my $date = undef;
		
			# r_date=2010-01-01_meeting_id=4.html
			if( $file =~ m/r_date\=(\d+\-\d+\-\d+)_meeting_id\=(\d+)\.html/ ) {
				$date = $1;
				$stadium = "STADIUM_" . $2;
				$stadium = 	$STADIUMS_BY_ID->{$2} if( exists($STADIUMS_BY_ID->{$2}) );
			}
			else {
				print "Invalid file name: $file";
				return;
			}
	  
			unless( exists($races->{$stadium}) && exists($races->{$stadium}->{$date}) ) {
				print "Getting info for race in $date at the $stadium stadium ...\n";
				my $dayraces = undef;
				($dogs, $dayraces) = Dogs::HTMLParser::parse_html_day_future_races_table($dogs, $file);
				$races->{$stadium}->{$date} = $dayraces;
			}
			
			store_data($races, $dogs);
		}
	};
	
	if($@) {
		print "Exception: $@";
	}
	
	print "PROCESSING FILE $file DONE\n\n";
}


# PARSE DATA FROM HTML FILES INTO PERL STORABLE HASHS
if( $OPERATION eq 'PARSEFILES' ) {
	if( -d $file_or_dir ) {
		chdir($file_or_dir);
		$DIRMODE = 1;
	}	
		
	if( $DIRMODE ) {
		opendir ( DIR, '.' ) || die "Error in opening dir $file_or_dir\n";
		while( (my $filename = readdir(DIR))){
			next if $filename eq '.' or $filename eq '..';
			next if( $filename !~ /\.html$/ );
			process_file($filename);
		}
		closedir(DIR);
	}
	elsif( -f $file_or_dir ) {
		process_file($file_or_dir);		
	}
	
	exit 0;
}
# INSERT DATA FROM PERL STORABLE HASHS INTO ORACLE DATABASE
elsif( $OPERATION eq 'PROCESSDATA' ) {
	# Read data from the Storable Hashes files
	
	my ($races, $dogs) = load_data();
	
	Dogs::DBConnector::process_past_races_data($races);
	
	print "\n\n\n\n#######################################################################################################\n";
	print "########################                     ALL DONE                  ################################\n";
	print "#######################################################################################################\n";
	
	exit 0;
}
elsif( $OPERATION eq 'PARSEFUTUREFILES' ) {
	if( -d $file_or_dir ) {
		chdir($file_or_dir);
		$DIRMODE = 1;
	}
	
	my ($races, $dogs) = load_data();
		
	if( $DIRMODE ) {
		opendir ( DIR, '.' ) || die "Error in opening dir $file_or_dir\n";
		while( (my $filename = readdir(DIR))){
			next if $filename eq '.' or $filename eq '..';
			next if( $filename !~ /\.html$/ );
			process_future_file($races, $dogs, $filename);
		}
		closedir(DIR);
	}
	elsif( -f $file_or_dir ) {
		process_future_file($races, $dogs, $file_or_dir);		
	}
	
	exit 0;
}
elsif( $OPERATION eq 'PROCESSFUTUREDATA' ) {
	# Read data from the Storable Hashes files
	
	my ($races, $dogs) = load_data();
	
	Dogs::DBConnector::process_future_races_data($races);
	
	print "\n\n\n\n#######################################################################################################\n";
	print "########################                     ALL DONE                  ################################\n";
	print "#######################################################################################################\n";
	
	exit 0;
}
# PRINT DATA FROM PERL STORABLE HASHS
elsif( $OPERATION eq 'DUMPDATA' ) {
	my ($races, $dogs) = load_data();
	
	print Dumper($races);
	print Dumper($dogs);
	
	exit 0;
}

1;