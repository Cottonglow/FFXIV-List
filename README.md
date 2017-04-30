# FFXIV-List
CI346 - Assignment
<p>FFXIVList offers the possibility to track your in game progress for Final Fantasy XIV externally allowing for a better overview and faster searching capabilities.</p>
<p>As it is difficult to gauge how much you have done and how much is left, you can use FFXIVList to track your progress. </p>
<p>This website has been created for the purpose of an assignment for university.</p>

<h3>Instructions</h3>
<h4>Building</h4>
<ul>
<li> Open command line. </li>
<li> Change directory to the ffxivList directory. </li>
cd "FFXIV-List\ffxivList"
<li> Enter the dotnet publish command </li>
dotnet publish -o publish -c Debug
<li> Change directory to the publish directory. </li>
cd publish
<li> Enter the dotnet run dll command </li>
dotnet ffxivList.dll
<li> Open a browser on http://localhost:5000</li>
<li> Shut down website server when done in command line </li>
</ul>
<h4>Testing</h4>
<ul>
<li> Open command line. </li>
<li> Change directory to the ffxivList.Tests directory. </li>
cd "FFXIV-List\ffxivList.Tests"
<li> Enter the dotnet test command </li>
dotnet test
</ul>
