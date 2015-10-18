set -o verbose

git rm -r --cached . 
git add .
git commit -m 'File tracking refreshed according to .gitignore' 
git push origin master

read -p "Press any key to exit.."