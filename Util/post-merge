#!/bin/perl
use File::Spec::Functions;
use File::Basename;
use File::Copy;
use Cwd 'realpath';

require '.git/hooks/functions.pl';

$DBPATH = getDBDir();
$searchString = getSearchString();

# Every time you pull, copy all art from Dropbox (if they're newer there)
$ASSETSDIR = catdir($DBPATH, "Assets");
`cp -r \"$ASSETSDIR\" .`;

`git ls-files $searchString | tr '\\n' '\\0' | xargs -0 git update-index --assume-unchanged`;