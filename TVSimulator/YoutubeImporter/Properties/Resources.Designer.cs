﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace YoutubeImporter.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("YoutubeImporter.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!DOCTYPE html&gt;
        ///&lt;html&gt;
        ///	&lt;head&gt;
        ///	&lt;meta charset=&apos;UTF-8&apos; /&gt;
        ///	&lt;style type=&apos;text/css&apos;&gt;
        ///		.videoWrapper {
        ///
        ///		}
        ///		.videoWrapper iframe {
        ///			position: absolute;
        ///			top: 0;
        ///			left: 0;
        ///			width: 100%;
        ///			height: 100%;
        ///		}
        ///	&lt;/style&gt;
        ///	&lt;/head&gt;
        ///	&lt;body&gt;
        ///	&lt;div class=&quot;videoWrapper&quot;&gt;
        ///		&lt;div id=&quot;player&quot;&gt;&lt;/div&gt;
        ///	&lt;/div&gt;
        ///
        ///	&lt;script src=&quot;CefPlayer.js&quot;&gt;&lt;/script&gt;
        ///	&lt;/body&gt;
        ///&lt;/html&gt;.
        /// </summary>
        internal static string CefPlayer {
            get {
                return ResourceManager.GetString("CefPlayer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to // 2. This code loads the IFrame Player API code asynchronously.
        ///  var tag = document.createElement(&apos;script&apos;);
        ///
        ///  tag.src = &quot;https://www.youtube.com/iframe_api&quot;;
        ///  var firstScriptTag = document.getElementsByTagName(&apos;script&apos;)[0];
        ///  firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);
        ///
        ///  // 3. This function creates an &lt;iframe&gt; (and YouTube player)
        ///  //    after the API code downloads.
        ///  var player;
        ///  var autoPlay = false;
        ///  var quality = &quot;hd720&quot;;
        ///  var startUpId = &apos;XIMLoLxmTDw&apos;;
        ///  
        ///  function onYouTub [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string CefPlayer1 {
            get {
                return ResourceManager.GetString("CefPlayer1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;html&gt;
        ///	&lt;head&gt;
        ///	&lt;meta charset=&apos;UTF-8&apos; /&gt;
        ///	&lt;style type=&apos;text/css&apos;&gt;
        ///	body {
        ///		overflow:hidden;
        ///	}
        ///	#player{height:95%; width: 100%;}
        ///	&lt;/style&gt;
        ///	&lt;/head&gt;
        ///	&lt;body&gt;
        ///	&lt;div id=&apos;player&apos;&gt;&lt;/div&gt;
        ///		&lt;script type=&apos;text/javascript&apos; src=&apos;http://www.youtube.com/player_api&apos;&gt;&lt;/script&gt;
        ///		&lt;script type=&apos;text/javascript&apos;&gt;
        ///			//holds on to player object
        ///			var player;
        ///
        ///			//create youtubeplayer
        ///			function onYouTubePlayerAPIReady() {
        ///				player = new YT.Player(&apos;player&apos;, {
        ///					height: &apos;100px&apos;,
        ///					width: &apos;100px&apos;,	
        ///					videoId: js [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Player {
            get {
                return ResourceManager.GetString("Player", resourceCulture);
            }
        }
    }
}
