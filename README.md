Loadify
=======
Loadify is a Spotify downloader written in .NET for downloading playlists and tracks from Spotify.
Please keep in mind that the software is currently a work-in-progress. Please let us know if something does not work as expected or if you find the documentation is lacking information.

Problem
-
Spotify offers a special mode, the offline mode. Using this offline mode, you are able to download tracks by storing it locally on your device for listening to music even if you don't have a connection to the internet.

Well, why would you use a software to download tracks if you could simply do that by using the official client? In fact, you just could use the official client and use that offline mode mentioned before but it comes with a huge disadvantage: The only way to listen to downloaded tracks is to use the official Spotify client. Since Spotify encrypts their audio data, you won't be able to listen to the music you've downloaded once your Spotify premium membership has expired.


Solution
-
There isn't much to say about Loadify. It is (yet another) Spotify downloader that is open source - useable for everyone. We have provided some features that we personally found important when it comes to downloading music. But, how does it work?

Since audio files supplied by the client are encrypted, we needed to jump in a little bit earlier and thus used **libspotify**, the official Spotify library written in C (we actually use a C# wrapper that manages the transition from unmanaged to managed by marshalling). Since audio data is streamed into some type of callback once we told the API to load a certain track into the audio player, we just need to capture the data flying into this callback. The track is currently saved as `wave` file into the specified folder and then immediately gets converted to a `MP3` file.


Contributions
-
Feel free to contribute by adding new features or translating Loadify into your native language.
Don't know how to help with translating? No problem, we've set up a [wiki page](https://github.com/Mostey/loadify/wiki/Translations-and-Localization)  
that should help you with getting started. Please create an issue if you've any questions.


Features
-

### User friendly, simple, beautiful
While this is not important for many other developers that did take hand on designing Spotify downloaders, it was very important for us to make using the software self-explainable and to keep things simple.

***

### Login and Authentication
The login is as simple as it gets. You just enter your username and password you'd normally use for logging into Spotify and click on `Login`. Please note that the _Remember me_ option works fine but requires to store your password unencrypted into the configuration file. If you can't deal with that, please don't use this feature.

![](http://i.epvpimg.com/b6zWd.png)


***

### Dashboard
After logging in, a new window containing your dashboard will open up. The software will ask you on the first startup if you want to fetch your playlists and displays them in the left pane.

<a href="url"><img src="http://i.epvpimg.com/h6Nxc.png" align="center" height="100%" width="700" ></a>

<a href="url"><img src="http://i.epvpimg.com/foUGe.png" align="center" height="100%" width="700" ></a>

<a href="url"><img src="http://i.epvpimg.com/hWPYc.png" align="center" height="100%" width="700" ></a>

The right pane is mainly used for configuration and settings, see the next section for detailed explanations of the available settings.

Once you've selected some tracks (or whole playlists) for downloading, the `Download` button will be enabled that triggers the download contract once it has been clicked.

<a href="url"><img src="http://i.epvpimg.com/vTMZd.png" align="center" height="100%" width="700" ></a>

***

### Settings

There are currently multiple settings implemented that can be used to customize Loadify:

#### Directories
* **Download Directory** - Base directory in which downloaded tracks will be stored
* **Cache Directory** - Directory in which cache files for speeding up the login/playlist fetching process are stored

#### Behavior
* **Local Track Detection**
  * **Ask me for removing tracks that were detected to be already downloaded** - If you select whole playlists, Loadify will ask you whether you want to remove the selection of those tracks you've already downloaded
* **File Management**
  * **Remove uncompressed audio files after they were converted** - After downloading, the uncompressed audio file is usually converted and then deleted. If this setting is unchecked, the uncompressed audio file will not be deleted
* **Download**
  * **Skip tracks that can not be downloaded automatically** - Sometimes tracks can't be downloaded due to unknown reasons, the official Spotify bridge does not provide any information about this incident but in case this happens, Loadify will continue downloading the next track automatically instead of showing the dialog asking what to do.
* **Other**
  * **Download and show playlist metadata on startup** - If checked, Loadify will download playlist metadata (such as name, tracks, artists etc) and displays them in the left panel of the dashboard. You can, however, refresh/load this data any time by right-clicking the left panel and clicking __Refresh__.

#### Language
* User Interface Language - Pretty much does what you expect. Implemented languages:
  * English
  * German


***

### Resizable Panes
Playlist or track names are too long to be fully displayed? No problem, you can easily use the slider to adjust the size of both panes.

As you might have found out, just grab the green bar with the dots on it and drag it to the left or right, depending on which panel you expect to be larger.

<a href="url"><img src="http://i.epvpimg.com/nW5Ff.png" align="center" height="100%" width="700" ></a>


***

### Tracks and Playlists
Once you expand a playlist in the left pane, all associated tracks will be listed in the following format:

`<Artists> - <Track name>`

If you hover over the tracks, a tooltip will display the track duration.

<a href="url"><img src="http://i.epvpimg.com/kEbzf.png" align="center"></a>

Each time you select a track, the software calculates the approximate time that is needed for downloading all of the selected tracks. The estimated time is displayed below the playlist/track listings.

You might have also noted the red crosses before each track listing. This is an indicator that signals if the track already exists on the local file system in the specified download directory.


***

### Downloading
Once you have started the download contract, the download status bar in the lower left corner will become visible informing you about the current download status:

* The progress bar represents the status of the track being download.
* The drawing right from the progress bar contains the name of the track being downloaded
* (n/x) right from the drawing represents the status of the download contract. **n** being the current track index and **x** being the amount of tracks that the software was contracted with.

<a href="url"><img src="http://i.epvpimg.com/SwVnf.png" align="center" height="100%" width="700"></a>


***


### Local Track detection
Once you've downloaded some tracks, you probably don't want to download them again on the next time you use Loadify. And since you won't have the time to check each file on your file system if you've already downloaded that particular track, Loadify will do this automatically for you.

Tracks will be detected as existing if:
* the filename matches the output format of music files converted by Loadify, i.e. `<Artists> - Track name`, as mentioned above
* the format of the file is the format currently used by Loadify (mp3 is the only format at the moment and used by default)
* the file is located within the very own subdirectory of the playlist. If I have my download directory located under `D:\Download` and my associated playlist is called `DnB`, I would need to have the directory `D:\Download\DnB` where the music files are stored in. Don't worry, Loadify will also do this automatically.

If a red cross is shown, the track does not exist (or simply wasn't found). If a green tick is shown, the track exists locally. If you, for example, select a whole playlist to download, the software will ask you whether you want to remove the existing tracks from your contract or not.

<a href="url"><img src="http://i.epvpimg.com/Eym1h.png" align="center" height="100%" width="650"></a>


***

### Searching for tracks
Sometimes you don't want to browse your playlists for a certain track you want to download. For quickly searching and finding tracks in your playlists, the search field was implemented:

Typing in __Eminem__ for example, will display all tracks that got the word __Eminem__ in their name somehow. (case insensitive)

Pressing enter will apply the search filter, removing the search text and pressing enter again will restore all playlists without the search filter.

<a href="url"><img src="http://i.epvpimg.com/Jruih.png" align="center"></a>


***

### Adding additional playlists and tracks
If you want to download playlists/tracks that you've not added to your spotify account, you can temporarily or permanently add them in Loadify to select them for the download contract.

#### Playlists
Right click in an empty area of the panel where the playlists and tracks are shown and select __Add Playlist__

<a href="url"><img src="http://i.epvpimg.com/bXmhd.png" align="center"></a>

A dialog will be displayed, prompting you to enter the link to the playlist you want to add. There are 2 types of playlist links:
* HTTP links (example: __http://open.spotify.com/user/spotify_germany/playlist/0QUQf1xMMbtArIbDjwi2Hf__)
* Spotify links (example: __spotify:user:spotify_germany:playlist:0QUQf1xMMbtArIbDjwi2Hf__)

Note the `/playlist/` and `:playlist:` section of the url.
If the url provided is a valid url to a playlist, Loadify will fetch all of it's tracks and add the new playlist in the left panel.

<a href="url"><img src="http://i.epvpimg.com/XWaxf.png" align="center" height="100%" width="650"></a>

Straight after entering the url, Loadify will ask you if you want the playlist to be added permanently to your Spotify account. If you proceed by clicking __yes__, the playlist will be permanently added and should also show up in your Spotify client.

<a href="url"><img src="http://i.epvpimg.com/9j3Qd.png" align="center" height="100%" width="700"></a>

Once you refresh your playlists, it will be removed if you choose to not permanently add it to your account.


#### Tracks
For adding single tracks into an existing playlist, you need to right click one of the playlists and select __Add Track__.

<a href="url"><img src="http://i.epvpimg.com/VCQof.png" align="center"></a>

This is basically following the same procedure as the __Add Playlist__ feature explained earlier. The only difference lies in the urls.

While playlist urls contain a `/playlist/` and `:playlist:` section, tracks do not. This section is replaced with the string `/track/` or `:track:`:

* HTTP links (example: __http://open.spotify.com/track/1B20QutZwNPRrlQ2tDKuOe__)
* Spotify links (example: __spotify:track:1B20QutZwNPRrlQ2tDKuOe__)

<a href="url"><img src="http://i.epvpimg.com/CvFcg.png" align="center" height="100%" width="650"></a>


***

### Other screenshots

<a href="url"><img src="http://i.epvpimg.com/JGkeh.png"></a>

<a href="url"><img src="http://i.epvpimg.com/0iLid.png" align="center" height="100%" width="600"></a>
