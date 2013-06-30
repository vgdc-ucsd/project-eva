#!/bin/perl
use File::Spec::Functions;
use Cwd 'realpath';

print "Before we continue, I need the absolute path\nto your project-eva Dropbox folder.\n";
print "EX: C:\\Users\\Alex\\Dropbox\\project-eva\\\n";

$found = 0;
while($found < 1) {
	print "PATH:\t";
	$DBPATH = <STDIN>;
	chomp($DBPATH);
	$DBPATH = realpath($DBPATH);

	if( -d $DBPATH ) {
		$found += 1;
	} else {
		print "\nSorry, I couldn't find that path. Try again";
	}
}

print "\nCool, thanks. This'll just take a second.\n";
`touch 'box.conf'`;
`echo "$DBPATH">>box.conf`;

print "Initializing git\n";
`git init .`;

print "Setting up hooks\n";
`mv functions.pl ./.git/hooks/`;
`mv post-commit ./.git/hooks/`;
`mv pre-commit ./.git/hooks/`;
`mv post-merge ./.git/hooks/`;

print "Pulling from remote\n";
system('git remote add origin git@github.com:ucsdvgdc/project-eva.git');
system('git checkout -b develop');
system('git pull origin develop');

print "Syncing with Dropbox\n";
$ASSETSDIR = catdir($DBPATH, "Assets");
system("cp -r \"$ASSETSDIR\" .");
`git ls-files | tr '\\n' '\\0' | xargs -0 git update-index --assume-unchanged`;
`rm setupRepo.pl`;

print "\nAll done!\n";