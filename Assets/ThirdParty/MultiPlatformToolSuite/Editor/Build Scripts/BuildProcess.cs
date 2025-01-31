using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System;

/*
 * BuildProcess
 * Owlchemy Labs, LLC - 2011
 * 
 * Menu-item driven build pipeline
 * Comments and instructions are placed in-line 
*/
public class BuildProcess : ScriptableObject
{
	//=====
	//CHANGE ME
	//Here we're specifying which scenes should go in which builds - add paths to all of your scenes in the arrays below.
	//Sometimes you want to include different scenes with different platforms, such as removing the level editor on Webplayer for example
	//=====
	static string[] standaloneScenes = new string[] { @"Assets/MultiPlatformToolSuite/testScene.unity" };
	static string[] iPhoneScenes = standaloneScenes;
	static string[] iPadScenes = standaloneScenes;
	static string[] webPlayerScenes = standaloneScenes;
	static string[] androidScenes = standaloneScenes;
	static string[] flashPlayerScenes = standaloneScenes;
	static string[] naClScenes = standaloneScenes;
#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1
	static string[] winPhoneScenes = standaloneScenes;
	static string[] windows8Scenes = standaloneScenes;
#endif
	static string[] levels; //This will be populated with the proper scenes for the chosen platform!
	static char delim = System.IO.Path.DirectorySeparatorChar;
	static string err = string.Empty;

	//=====
	//CHANGE ME - Make sure to set your bundle identifier and game name(s)!
	//=====
	const string bundleIdentifier = "com.CompanyName.GameName";

	const string genericBuildName = "GameName";
	const string genericProductName = "GameName";
	
	const string macBuildName = genericBuildName + ".app";
	const string macProductName = genericProductName;

	const string winBuildName = genericBuildName + ".exe";
	const string winProductName = genericProductName;

	const string webBuildName = genericBuildName;
	const string webProductName = genericProductName;

	const string iPhoneBuildName = genericBuildName;
	const string iPhoneProductName = genericProductName;

	const string iPadBuildName = genericBuildName;
	const string iPadProductName = genericProductName;
	
	const string androidBuildName = genericBuildName;
	const string androidProductName = genericProductName;
	
	const string flashPlayerBuildName = genericBuildName + ".swf";
	const string flashPlayerProductName = genericProductName;
	
	const string naClBuildName = genericBuildName;
	const string naClProductName = genericProductName;
	
	const string winPhoneBuildName = genericBuildName;
	const string winPhoneProductName = genericProductName;
	
	const string windows8BuildName = genericBuildName;
	const string windows8ProductName = genericProductName;

	//=====
	//Below are the menu items under the Build menu. Feel free to add to this list to achieve custom build processes
	//Examples include a debug build, a press build, a profiler build, a demo build, etc.
	//=====
	[MenuItem("Window/MultiPlatform ToolKit/Build/Build iPhone", false, 1)]
	public static void BuildNonDebugiPhone ()
	{
		//Make platform-specific changes specifically for iPhone
		PlatformSpecificChanges (Platform.iPhone);
		
		//Finish the rest of the build process for iPhone
		BuildiPhone (false, iPhoneBuildName, iPhoneProductName);
	}

	[MenuItem("Window/MultiPlatform ToolKit/Build/Build iPad", false, 1)]
	public static void BuildNonDebugiPad ()
	{
		PlatformSpecificChanges (Platform.iPad);
		BuildiPad (false, iPadBuildName, iPadProductName);
	}
	
	[MenuItem("Window/MultiPlatform ToolKit/Build/Build Universal iOS", false, 1)]
	public static void BuildNonDebugUniversaliOS ()
	{
		PlatformSpecificChanges (Platform.iPad, false);
		BuildiPad (false, iPadBuildName, iPadProductName);
	}

	[MenuItem("Window/MultiPlatform ToolKit/Build/Build Mac Intel", false, 1)]
	public static void BuildMacIntelOnly ()
	{
		PlatformSpecificChanges (Platform.Standalone);
		BuildMac (false, macBuildName, macProductName);
	}

	[MenuItem("Window/MultiPlatform ToolKit/Build/Build Windows", false, 1)]
	public static void BuildWindowsOnly ()
	{
		PlatformSpecificChanges (Platform.Standalone);
		BuildWin (false, winBuildName, winProductName);
	}

	[MenuItem("Window/MultiPlatform ToolKit/Build/Build Web", false, 1)]
	public static void BuildWeb ()
	{
		PlatformSpecificChanges (Platform.WebPlayer);
		BuildWeb (false, webBuildName, webProductName);
	}
	
	[MenuItem("Window/MultiPlatform ToolKit/Build/Build Android", false, 1)]
	public static void BuildNonDebugAndroid ()
	{
		PlatformSpecificChanges (Platform.Android);
		BuildAndroid (false, androidBuildName, androidProductName);
	}

	[MenuItem("Window/MultiPlatform ToolKit/Build/Build FlashPlayer", false, 1)]
	public static void BuildNonDebugFlashPlayer ()
	{
		PlatformSpecificChanges (Platform.FlashPlayer);
		BuildFlashPlayer (false, flashPlayerBuildName, flashPlayerProductName);
	}
	
	[MenuItem("Window/MultiPlatform ToolKit/Build/Build NaCl", false, 1)]
	public static void BuildNonDebugNaCl ()
	{
		PlatformSpecificChanges (Platform.NaCl);
		BuildNaCl (false, naClBuildName, naClProductName);
	}
	
#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1
	[MenuItem("Window/MultiPlatform ToolKit/Build/Build WP8", false, 1)]
	public static void BuildNonDebugWinPhone ()
	{
		PlatformSpecificChanges (Platform.WP8);
		BuildWinPhone (false, winPhoneBuildName, winPhoneProductName);
	}
	
	[MenuItem("Window/MultiPlatform ToolKit/Build/Build Windows8", false, 1)]
	public static void BuildNonDebugWindows8 ()
	{
		PlatformSpecificChanges (Platform.Windows8);
		BuildWindows8 (false, windows8BuildName, windows8ProductName);
	}
#endif
	
	//=====
	//DEBUG build options
	//=====
	[MenuItem("Window/MultiPlatform ToolKit/Build/Debug/Build iPhone", false, 1)]
	public static void BuildDebugiPhone ()
	{
		PlatformSpecificChanges (Platform.iPhone);
		BuildiPhone (true, iPhoneBuildName, iPhoneProductName);
	}

	[MenuItem("Window/MultiPlatform ToolKit/Build/Debug/Build iPad", false, 1)]
	public static void BuildDebugiPad ()
	{
		PlatformSpecificChanges (Platform.iPad);
		BuildiPad (true, iPadBuildName, iPadProductName);
	}
	
	[MenuItem("Window/MultiPlatform ToolKit/Build/Debug/Build Android", false, 1)]
	public static void BuildDebugAndroid ()
	{
		PlatformSpecificChanges (Platform.Android);
		BuildAndroid (true,androidBuildName, androidProductName);
	}

	[MenuItem("Window/MultiPlatform ToolKit/Build/Debug/Build FlashPlayer", false, 1)]
	public static void BuildDebugFlashPlayer ()
	{
		PlatformSpecificChanges (Platform.FlashPlayer);
		BuildFlashPlayer (true,flashPlayerBuildName, flashPlayerProductName);
	}
	
	[MenuItem("Window/MultiPlatform ToolKit/Build/Debug/Build NaCl", false, 1)]
	public static void BuildDebugNaCl ()
	{
		PlatformSpecificChanges (Platform.NaCl);
		BuildNaCl (true,naClBuildName, naClProductName);
	}

#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1
	[MenuItem("Window/MultiPlatform ToolKit/Build/Debug/Build WP8", false, 1)]
	public static void BuildDebugWinPhone ()
	{
		PlatformSpecificChanges (Platform.WP8);
		BuildWinPhone (true, winPhoneBuildName, winPhoneProductName);
	}
	
	[MenuItem("Window/MultiPlatform ToolKit/Build/Debug/Build Windows8", false, 1)]
	public static void BuildDebugWindows8 ()
	{
		PlatformSpecificChanges (Platform.Windows8);
		BuildWindows8 (true, windows8BuildName, windows8ProductName);
	}
#endif
	
	//=====
	//Run through all of the scenes (working on duplicates) and make changes specified in PlatformSpecifics BEFORE building
	//=====
	public static void PlatformSpecificChanges (Platform platform, bool stripMaterials) {
		//Set platform override to the specified platform during the platform-specific changes, but revert back
		//to the platform that was originally set at the end
		string previousPlatformOverride = EditorPrefs.GetString (Platforms.editorPlatformOverrideKey, "Standalone");
		EditorPrefs.SetString (Platforms.editorPlatformOverrideKey, platform.ToString ());
		
		//Make sure to use the proper array of scenes, specified at the top of the file
		if (platform == Platform.WebPlayer)
			levels = webPlayerScenes; 
		else if (platform == Platform.iPhone)
			levels = iPhoneScenes; 
		else if (platform == Platform.iPad)
			levels = iPadScenes;
		else if (platform == Platform.Android)
			levels = androidScenes;
		else if (platform == Platform.FlashPlayer)
			levels = flashPlayerScenes;
		else if (platform == Platform.NaCl)
			levels = naClScenes;	
#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1
		else if (platform == Platform.WP8)
			levels = winPhoneScenes;	
		else if (platform == Platform.Windows8)
			levels = windows8Scenes;			
#endif
		else
			levels = standaloneScenes;
		
		//=====
		//First, save the current scene!
		//WARNING Running a build will save your current scene
		//This is 'destructive' if you're not using version control or didnt want to save.
		//=====
		EditorApplication.SaveScene (EditorApplication.currentScene);
		
		//Duplicate the scenes to work on them, then we can restore the originals later on
		foreach (string scenePath in levels) {
			string newPath = scenePath.Insert (scenePath.IndexOf (".unity"), "_temp");
			//Rename the original scenes with a _temp suffix
			AssetDatabase.MoveAsset (scenePath, newPath);
			//Rename original scene file
			AssetDatabase.CopyAsset (newPath, scenePath);
			//Create duplicate file that uses the original scene file name
		}
		
		//Loop through each scene
		foreach (string scenePath in levels) {
			//Open the scene
			EditorApplication.OpenScene (scenePath);
			Debug.Log ("Processing scene: " + scenePath);
			
			//Find all instances of PlatformSpecifics and apply the changes specified therein
			PlatformSpecifics[] specifics = FindObjectsOfType (typeof(PlatformSpecifics)) as PlatformSpecifics[];
			ApplyPlatformSpecifics (specifics, platform, stripMaterials);
			
			//Save, then do it again!
			EditorApplication.SaveScene (scenePath);
			AssetDatabase.Refresh ();
		}
		
		//Set the Editor to return to the first scene in the build settings, now that we're done combing through the scenes
		EditorApplication.OpenScene (levels[0]);
		
		//Revert platform override to what was previously set
		EditorPrefs.SetString (Platforms.editorPlatformOverrideKey, previousPlatformOverride);
	}

	//Helper function for PlatformSpecificChanges
	public static void PlatformSpecificChanges (Platform platform)
	{
		PlatformSpecificChanges(platform, true);
	}

	//Apply the changes specified in the PlatformSpecifics instance
	static void ApplyPlatformSpecifics (PlatformSpecifics[] specifics, Platform platform, bool stripMaterials)
	{
		//For every platform
		foreach (PlatformSpecifics instance in specifics) {
			//Apply the specific changes
			instance.ApplySpecifics (platform);
			
			//Once we've applied, lets remove referenced materials to make sure we don't include unnecessary materials on a build
			if (stripMaterials && instance != null)
				instance.materialPerPlatform = null;
		}
	}

	//=====
	//Build processes for each platform, run after the PlatformSpecificChanges are done
	//=====
	public static void BuildiPad (bool debug, string buildName, string productName)
	{
		//Get the build directory - by default it's the project's root directory
		string buildPath = GetBuildDirectory ().FullName + delim + buildName;
		
		//Currently we've configured this example project to create separate iPad builds from iPhone builds 
		//instead of a universal build
		BuildTarget target = BuildTarget.iPhone;
		//iPhone here really means 'iOS'
		PlayerSettings.productName = productName;
		//product name should have been set above!
		PlayerSettings.iOS.targetDevice = iOSTargetDevice.iPadOnly;
		//lets make an iPad only build
		PlayerSettings.iOS.targetResolution = iOSTargetResolution.Native;
		//native iPad resolution
		PlayerSettings.bundleIdentifier = bundleIdentifier;
		//bundle identifier should have been set above!
		PlayerSettings.iOS.applicationDisplayName = productName;
		//you can change the display name here (in the springboard on-device)
		PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
		//lets default to Landscape Left
		PlayerSettings.allowedAutorotateToLandscapeLeft = false;
		//lets also block all other orientations!
		PlayerSettings.allowedAutorotateToLandscapeRight = false;
		PlayerSettings.allowedAutorotateToPortrait = false;
		PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
		//There's a whole bunch of other optional PlayerSettings that can be automatically set here! Check out the API.
		
		//Pre-process the build
		PreProcessiOSBuild ();
		
		//Actually execute the build!
		if (!debug)
			err = BuildPipeline.BuildPlayer (levels, buildPath, target, BuildOptions.AutoRunPlayer);
		else
			err = BuildPipeline.BuildPlayer (levels, buildPath, target, BuildOptions.Development | BuildOptions.ConnectWithProfiler);
		
		//Post-process the build
		if (err != string.Empty) {
			Debug.Log (err);
		} else {
			PostProcessiOSBuild (buildPath);
		}
		
		//Finally, revert back to the scene files that were present at the beginning of the build process
		RevertScenes ();
	}

	public static void BuildiPhone (bool debug, string buildName, string productName)
	{
		string buildPath = GetBuildDirectory ().FullName + delim + buildName;
		BuildTarget target = BuildTarget.iPhone;
		PlayerSettings.productName = productName;
		PlayerSettings.iOS.targetDevice = iOSTargetDevice.iPhoneOnly;
		PlayerSettings.iOS.targetResolution = iOSTargetResolution.Native;
		PlayerSettings.bundleIdentifier = bundleIdentifier;
		PlayerSettings.iOS.applicationDisplayName = productName;
		PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
		
		PreProcessiOSBuild ();
		
		if (!debug)
			err = BuildPipeline.BuildPlayer (levels, buildPath, target, BuildOptions.AutoRunPlayer);
		else
			err = BuildPipeline.BuildPlayer (levels, buildPath, target, BuildOptions.Development | BuildOptions.ConnectWithProfiler);
		
		if (err != string.Empty) {
			Debug.Log (err);
		} else {
			PostProcessiOSBuild (buildPath);
		}
		
		RevertScenes ();
	}
	
	public static void BuildAndroid (bool debug, string buildName, string productName)
	{
		string buildPath = GetBuildDirectory ().FullName + delim + buildName;
		BuildTarget target = BuildTarget.Android;
		PlayerSettings.productName = productName;
		PlayerSettings.Android.targetDevice = AndroidTargetDevice.ARMv7;
		
		//remember to set your Android bundle version code higher each time.
		//see the Bundle Version Code docs here: http://unity3d.com/support/documentation/Components/class-PlayerSettings.html
		PlayerSettings.Android.bundleVersionCode = 1; 
		
		PlayerSettings.bundleVersion = "1.0";
		PlayerSettings.bundleIdentifier = bundleIdentifier;
		PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
		
		//You may choose to pre-process your Android build here.
		
		if (!debug)
			err = BuildPipeline.BuildPlayer (levels, buildPath, target, BuildOptions.AutoRunPlayer);
		else
			err = BuildPipeline.BuildPlayer (levels, buildPath, target, BuildOptions.Development | BuildOptions.ConnectWithProfiler);
		
		if (err != string.Empty) {
			Debug.Log (err);
		} else {
			//You may choose to post-process your Android build here.
		}
		
		RevertScenes ();
	}

	public static void BuildFlashPlayer (bool debug, string buildName, string productName)
	{
		string buildPath = GetBuildDirectory ().FullName + delim + buildName;
		BuildTarget target = BuildTarget.FlashPlayer;
		PlayerSettings.productName = productName;
		//More flash-specific player settings coming soon in the final v3.5!
		
		//You may choose to pre-process your Flash build here.
		
		if (!debug)
			err = BuildPipeline.BuildPlayer (levels, buildPath, target, BuildOptions.AutoRunPlayer);
		else
			err = BuildPipeline.BuildPlayer (levels, buildPath, target, BuildOptions.Development | BuildOptions.ConnectWithProfiler | BuildOptions.AutoRunPlayer);
		
		if (err != string.Empty) {
			Debug.Log (err);
		} else {
			//You may choose to post-process your Flash build here.
		}
		
		RevertScenes ();
	}
	
	public static void BuildNaCl (bool debug, string buildName, string productName)
	{
		string buildPath = GetBuildDirectory ().FullName + delim + buildName;
		BuildTarget target = BuildTarget.NaCl;
		PlayerSettings.productName = productName;
		
		//offline testing support
		//=====
		//CHANGE ME when you want to disable offline deployment
		//=====
		EditorUserBuildSettings.webPlayerOfflineDeployment = true;
	
		//You may choose to pre-process your NaCl build here.
		
		//enable NaCl support
		#if UNITY_3_5
		EditorUserBuildSettings.webPlayerNaClSupport = true;
		#endif
		
		//You may choose to pre-process your NaCl build here.

		if (!debug)
			err = BuildPipeline.BuildPlayer (levels, buildPath, target, BuildOptions.AutoRunPlayer);
		else
			err = BuildPipeline.BuildPlayer (levels, buildPath, target, BuildOptions.Development | BuildOptions.ConnectWithProfiler | BuildOptions.AutoRunPlayer);
		
		if (err != string.Empty) {
			Debug.Log (err);
		} else {
			//You may choose to post-process your NaCl build here.
		}
		
		RevertScenes ();
	}
	
#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1
	public static void BuildWinPhone (bool debug, string buildName, string productName)
	{
		string buildPath = GetBuildDirectory ().FullName + delim + buildName;
		
		//
		//uncomment these lines for various winPhone buildtypes
		//
		//BuildTarget target = BuildTarget.MetroPlayerARM;
		//BuildTarget target = BuildTarget.MetroPlayer;
		BuildTarget target = BuildTarget.StandaloneWindows;
		
		//then set this to true!
		bool doWinBuild = false;
		
		PlayerSettings.productName = productName;
		
		//You may choose to pre-process your WP8 build here.
		
		if(doWinBuild)
		{
			if (!debug)
				err = BuildPipeline.BuildPlayer (levels, buildPath, target, BuildOptions.AutoRunPlayer);
			else
				err = BuildPipeline.BuildPlayer (levels, buildPath, target, BuildOptions.Development | BuildOptions.ConnectWithProfiler | BuildOptions.AutoRunPlayer);
			
			if (err != string.Empty) {
				Debug.Log (err);
			} else {
				//You may choose to post-process your WP8 build here.
			}
		}
		else 
		{
			Debug.LogWarning("DoWinBuild is set to false - set me to true to enable building!");
		}
		
		RevertScenes ();
	}
	
	public static void BuildWindows8 (bool debug, string buildName, string productName)
	{
		string buildPath = GetBuildDirectory ().FullName + delim + buildName;
		
		//
		//uncomment these lines for various win8 buildtypes
		//
		//BuildTarget target = BuildTarget.MetroPlayerX86;
		//BuildTarget target = BuildTarget.MetroPlayer;
		//BuildTarget target = BuildTarget.MetroPlayerX64;
		//BuildTarget target = BuildTarget.WP8Player
		BuildTarget target = BuildTarget.StandaloneWindows;
		
		//then set this to true!
		bool doWinBuild = false;
		
		PlayerSettings.productName = productName;
		
		//You may choose to pre-process your Windows8 build here.
		
		if(doWinBuild)
		{
			if (!debug)
				err = BuildPipeline.BuildPlayer (levels, buildPath, target, BuildOptions.AutoRunPlayer);
			else
				err = BuildPipeline.BuildPlayer (levels, buildPath, target, BuildOptions.Development | BuildOptions.ConnectWithProfiler | BuildOptions.AutoRunPlayer);
			
			if (err != string.Empty) {
				Debug.Log (err);
			} else {
				//You may choose to post-process your Windows8 build here.
			}
		}
		else 
		{
			Debug.LogWarning("DoWinBuild is set to false - set me to true to enable building!");
		}
		RevertScenes ();
	}
#endif
	
	public static void BuildWeb (bool debug, string buildName, string productName)
	{
		string buildPath = GetBuildDirectory ().FullName + delim + buildName;
		BuildTarget target = BuildTarget.WebPlayer;
		
		PlayerSettings.productName = productName;
		
		err = BuildPipeline.BuildPlayer (levels, buildPath, target, BuildOptions.ShowBuiltPlayer);
		if (err != string.Empty)
			Debug.Log (err);
		
		RevertScenes ();
	}

	public static void BuildMac (bool debug, string buildName, string productName)
	{
		string buildPath = GetBuildDirectory ().FullName + delim + buildName;
		BuildTarget target = BuildTarget.StandaloneOSXIntel;
		
		
		PlayerSettings.productName = productName;
		PlayerSettings.defaultIsFullScreen = true;
		//There's a whole bunch of other optional PlayerSettings that can be automatically set here! Check out the API.
		
		err = BuildPipeline.BuildPlayer (levels, buildPath, target, BuildOptions.ShowBuiltPlayer);
		if (err != string.Empty)
			Debug.Log (err);
		
		RevertScenes ();
	}

	public static void BuildWin (bool debug, string buildName, string productName)
	{
		string buildPath = GetBuildDirectory ().FullName + delim + buildName;
		
		Debug.Log (buildPath);
		BuildTarget target = BuildTarget.StandaloneWindows;
		
		PlayerSettings.productName = productName;
		PlayerSettings.defaultIsFullScreen = true;
		
		err = BuildPipeline.BuildPlayer (levels, buildPath, target, BuildOptions.ShowBuiltPlayer);
		if (err != string.Empty)
			Debug.Log (err);
		
		RevertScenes ();
	}

	public static void PreProcessiOSBuild ()
	{
		//Do any pre-processing or moving around of files before the build happens, in here!
		//For example, we renamed a .dll file that was only for PC builds so that it wouldn't get included in the XCode project
		//This might be useful for some, so that code is included below (commented out for example purposes)
		
		//Rename PC_ONLY.dll file so it doesn't get built into XCode project
		//AssetDatabase.MoveAsset("Assets/Scripts/PC/PC_ONLY.dll", "Assets/Scripts/PC/PC_ONLY.txt");
	}

	public static void PostProcessiOSBuild (string buildDirectoryPath)
	{
		//=====
		//Example for how to postprocess the iOS Info.plist
		//=====
		ModifyInfoPlist ();
		
		//=====
		//Copy over all classes - useful for copying Objective C files into your XCode project
		//=====
//		DirectoryInfo classesDirectory = new DirectoryInfo(Application.dataPath + "/Editor/Build Resources/Classes");
//		FileInfo[] classFiles = classesDirectory.GetFiles();
//		foreach(FileInfo classFile in classFiles) {
//			classFile.CopyTo(buildDirectoryPath + "/Classes/" + classFile.Name, true);
//		}
		
		//=====
		//Next we're going to edit an AppleScript which will modify our XCode project for us
		//=====
//		//Write project path to postprocess apple script
//		Debug.Log("Writing project path to postprocess AppleScript.");
//		string appleScriptPath = Application.dataPath + "/Editor/Build Resources/PostProcessAppleScript.txt";
//		Debug.Log("AppleScript path: " + appleScriptPath);
//		string appleScript = File.ReadAllText(appleScriptPath);
//		List<string> lines = new List<string>(appleScript.Split('\n'));
//		//Remove the very last line since it'll be an empty one after the Split
//		lines.RemoveAt(lines.Count - 1);
//		
//		//Replace 3rd line to include project path
//		lines[2] = "	open \"" + buildDirectoryPath + "/Unity-iPhone.xcodeproj" + "\"";
//		File.WriteAllLines(appleScriptPath, lines.ToArray());
//		
//		//Add custom classes to XCode build
//		System.Diagnostics.Process process = new System.Diagnostics.Process();
//		process.StartInfo.FileName = "/usr/bin/osascript";
//		process.StartInfo.UseShellExecute = false;
//		process.StartInfo.RedirectStandardError = true;
//		process.StartInfo.RedirectStandardOutput = true;
//		process.StartInfo.Arguments = "\"" + appleScriptPath + "\"";
//		process.Start();
//		Debug.Log("Postprocess StdError: " + process.StandardError.ReadToEnd());
//		Debug.Log("Postprocess StdOutput: " + process.StandardOutput.ReadToEnd());
//		process.WaitForExit();
//		process.Dispose();
		
		//Undo the renaming we did in PreProcessiOSBuild()
//		AssetDatabase.MoveAsset("Assets/Scripts/PC/PC_ONLY.txt", "Assets/Scripts/PC/PC_ONLY.dll");
	}

	public static void ModifyInfoPlist ()
	{
		//=====
		//Modify the Info Plist
		//In this example, we're going to remove the Orientation entry for LandscapeLeft
		//since there was a bug in Unity 3.3 that wouldn't allow you to set a single orientation like this from within Unity
		//Including for example purposes
		//=====
		
//		DirectoryInfo buildDirectory = new DirectoryInfo(buildDirectoryPath); //Get build directory
//		FileInfo[] plistFiles = buildDirectory.GetFiles("Info.plist"); //Get Info.plist files in build directory
//		if(plistFiles == null || plistFiles.Length == 0) { //If none were found
//			Debug.Log("No Plist files were found in build directory."); //Log error
//			return;
//		}
//		FileInfo plist = plistFiles[0]; //Get the first Info.plist file
//		StreamReader readStream = plist.OpenText(); //Open Info.plist for reading
//		FileInfo newPlist = new FileInfo(buildDirectoryPath + Path.DirectorySeparatorChar + "Info_tmp.plist"); //Create a new Info_tmp.plist file
//		StreamWriter writeStream = newPlist.CreateText(); //Open Info_tmp.plist for writing
//		
//		string line = string.Empty;
//		
//		//Copy Info.plist to Info_tmp.plist line by line
//		//but skip the line that contains UIInterfaceOrientationLandscapeLeft
//		while((line = readStream.ReadLine()) != null) { 
//			if(line.Contains("UIInterfaceOrientationLandscapeLeft")) //If line contains LandscapeLeft orientation
//				continue; //Skip this line
//			writeStream.WriteLine(line); //Write line
//		}
//		
//		writeStream.Close(); //Close write stream
//		readStream.Close(); //Close read stream
//		
//		string filename = plist.FullName; //Get the filename of Info.plist
//		plist.Delete(); //Delete Info.plist
//		newPlist.MoveTo(filename); //Rename Info_tmp.plist to Info.plist
	}

	//=====
	//Get the build directory
	//By default, the build directory is in the project's base directory
	//If you have a "Builds" folder, it'll place builds in there (recommended to reduce clutter)
	//=====
	static DirectoryInfo GetBuildDirectory ()
	{
		DirectoryInfo assetsDirectory = new DirectoryInfo (Application.dataPath);
		DirectoryInfo projectDirectory = assetsDirectory.Parent;
		DirectoryInfo[] buildDirectories = projectDirectory.GetDirectories ("Builds");
		return (buildDirectories == null || buildDirectories.Length == 0) ? projectDirectory : buildDirectories[0];
	}

	//Revert the scenes back to normal, removing the _temp appended to the end
	public static void RevertScenes ()
	{
		foreach (string scenePath in levels) {
			string newPath = scenePath.Insert (scenePath.IndexOf (".unity"), "_temp");
			AssetDatabase.DeleteAsset (scenePath);
			AssetDatabase.MoveAsset (newPath, scenePath);
		}
		EditorApplication.OpenScene (levels[0]);
		
		Debug.Log ("Build complete!");
	}
}