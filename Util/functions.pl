#!/bin/perl
use File::Spec::Functions;
use File::Basename;
use File::Copy;
use Cwd 'realpath';

# UPDATE THIS IF WE EVER NEED MORE. DON'T FORGET PRE-COMMIT

sub touchConf
{
	# Check if dropbox has been configured
	if( ! -e 'box.conf' ) {
		# Create the conf file
		`touch box.conf`;
		# Check the default directory
		if( -d "~/dropbox/project-eva" ) {
			`echo ~/dropbox/project-eva >> project-eva`;
		} else {
			$FOUND = 0;
			print "I couldn't find your Dropbox folder, please enter\nthe absolute path to your project-eva folder in box.conf\n";
			exit 1;
		}
	}
}

sub getDBDir 
{
	open my $CONF, "box.conf" or die "Couldn't find your box.conf file!";
	$DBPATH = <$CONF>;
	chomp($DBPATH);
	$DBPATH = realpath($DBPATH);

	if( ! -d $DBPATH ) {
		print "I couldn't find your Dropbox folder. Did you edit box.conf?";
		exit 1;
	}
	return $DBPATH
}

sub getFileTypes
{
	# EDIT THIS IF WE EVER NEED MORE FILE TYPES IN HERE
	return ('.png', '.PNG', '.tga', '.TGA', '.fbx', '.FBX', '.mp3', '.MP3', '.ogg', '.OGG', '.mat', '.MAT');
}

sub getSearchString
{
	return "*" . join(' *', @FILETYPES);
}

1;