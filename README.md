Receive gitlab webhook push event, parse workpackage id(s) from commit message then add comment(s) to the mentioned workpackage(s) on OpenProject.

Note:
The commit message must be included workpackage id with '#' prefix. It looks like:

> This is my commit #23


You can include multiple workpackage id in commit message
> This is my commit #23 #43 #87

This will add comment to mentioned workpackages and comments look like:
> {author_name} ({author_email}) mentioned this work package in a commit {commit_message} of {repository_name}

And of course, you can change everything you want.
To make it works with your OpenProject, you need specify your OpenProject settings in **openproject.json**
