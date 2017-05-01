# FFXIV-List
CI346 - Assignment
<p>FFXIVList offers the possibility to track your in game progress for Final Fantasy XIV externally allowing for a better overview and faster searching capabilities.</p>
<p>As it is difficult to gauge how much you have done and how much is left, you can use FFXIVList to track your progress. </p>
<p>This website has been created for the purpose of an assignment for university.</p>

<h2>Debug Instructions</h2>
<p> This project has been built and tested on Windows 7 and Windows 10 on Firefox. </p>
<h3>Building</h3>
<ul>
<li> Download the .Net Core SDK from <a href="https://www.microsoft.com/net/core#windowscmd">the official microsoft page</a> or get the latest version straight from the <a href="https://github.com/dotnet/cli">github repository</a>.</li>
<li> Open command line. </li>
<li> Change directory to the ffxivList directory. </li>
<p>cd "FFXIV-List\ffxivList"</p>
<li> Enter the dotnet restore command to restore the dependencies and tools. </li>
<p> dotnet restore </p>
<li> Enter the dotnet publish command </li>
<p>dotnet publish -o publish -c Debug</p>
<li> Change directory to the publish directory. </li>
<p>cd publish</p>
<li> Enter the dotnet run dll command </li>
<p>dotnet ffxivList.dll</p>
<li> Open a browser on http://localhost:5000</li>
<li> Shut down website server when done in command line </li>
</ul>

<h3>Testing</h3>
<ul>
<li> Open command line. </li>
<li> Change directory to the ffxivList.Tests directory. </li>
<p>cd "FFXIV-List\ffxivList.Tests"</p>
<li> Enter the dotnet test command </li>
<p>dotnet test</p>
</ul>

<h2> Website Instructions</h2>

<h3> Login </h3>
<p>You are able to sign up with your own account (It will default to standard user.) or with one of the following from the in memory database:</p>
<ul>
<li> <b>Email:</b> admin@test.test <b>UserName:</b> admin2 <b>Password:</b> test <b>Role:</b> Admin </li>
<li> <b>Email:</b> user1@test.test <b>UserName:</b> user1 <b>Password:</b> test <b>Role:</b> Standard User</li>
</ul>
<p> <b> Note: </b> If you create your own account, when you quit your session, the in memory database will forget you, however you will still exist on the auth0 authorization database.</p>
<p> You may receive a security warning on logging in stating that the information you have entered will be sent over an insecure connection and could be read by a third party. </p>
<p> You're username and email will be used for profiling purposes as well as creating a database entry for you to be able to store your user progress. </p>

<h3> Authorization Levels </h3>
<p>The website has several authorization roles which offer a different user experience:</p>
<ul>
<li> <b>Admin</b> </li>
<p>Administrators are able to track their in game progress by checking checkboxes on their respective pages and view their profile as well as carry out any administrative needs on the website (e.g. creating, editing or deleting entries).</p>
<li> <b>Standard User</b> </li>
<p>A standard user is able to track their in game progress by checking checkboxes on their respective pages and view their profile.</p>
<li> <b>Anonymous User</b></li>
<p>An anonymous user is only able to view pages but not track their in game progress. They also do not have access to a user profile.</p>
</ul>

<h3> Saving Progress </h3>
<p>To start saving your in game progress, head over to the Levemetes, Quests or Crafts tabs available in the navigation bar after you have logged in.</p>
<p>Once you are there, you are able to check the relevant checkboxes and hit the Save button at the bottom of the screen to send your data to the database. You can instantly see your progress bar updating! </p>
<p>To see all your progress at once, check out your user profile.</p>

<h3> Admin </h3>
<p> After logging into an admin account (UserName: admin2, Password: test), head over to the Admin tab available in the navigation bar and select the entries you want to update. </p>

<h1> Future Enhancements </h1>
<ul>
<li> No longer trigger a security warning on login. </li>
<li> More information on error pages. </li>
<li> Be able to delete users on auth0 from code. </li>
</ul>
