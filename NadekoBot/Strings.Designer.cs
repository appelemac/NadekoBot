﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NadekoBot {
    using System;
    using System.Reflection;
    
    
    /// <summary>
    ///    A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        internal Strings() {
        }
        
        /// <summary>
        ///    Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("NadekoBot.Strings", typeof(Strings).GetTypeInfo().Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///    Overrides the current thread's CurrentUICulture property for all
        ///    resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to Could not initialize Credentials from credentials.json: {0}, quitting....
        /// </summary>
        public static string NadekoClient_InitializeCredentials_CouldNotInitializeCredentialsFromCredentialsJson0Quitting {
            get {
                return ResourceManager.GetString("NadekoClient_InitializeCredentials_CouldNotInitializeCredentialsFromCredentialsJs" +
                        "on0Quitting", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to Could not read Token, quitting....
        /// </summary>
        public static string NadekoClient_InitializeCredentials_CouldNotReadTokenQuitting {
            get {
                return ResourceManager.GetString("NadekoClient_InitializeCredentials_CouldNotReadTokenQuitting", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to Writing example of credentials failed: {0}.
        /// </summary>
        public static string NadekoClient_Main_WritingExampleOfCredentialsFailed0 {
            get {
                return ResourceManager.GetString("NadekoClient_Main_WritingExampleOfCredentialsFailed0", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to Connected.
        /// </summary>
        public static string NadekoClient_Start_Connected {
            get {
                return ResourceManager.GetString("NadekoClient_Start_Connected", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to Connecting to Discord....
        /// </summary>
        public static string NadekoClient_Start_ConnectingToDiscord {
            get {
                return ResourceManager.GetString("NadekoClient_Start_ConnectingToDiscord", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to Ready to receive commands.
        /// </summary>
        public static string NadekoClient_Start_ReadyToReceiveCommands {
            get {
                return ResourceManager.GetString("NadekoClient_Start_ReadyToReceiveCommands", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to New name must be allowed.
        /// </summary>
        public static string Utilities_RenameCommand_NewNameMustBeAllowed {
            get {
                return ResourceManager.GetString("Utilities_RenameCommand_NewNameMustBeAllowed", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to Owner-only command.
        /// </summary>
        public static string Utilities_RenameCommand_OwnerOnlyCommand {
            get {
                return ResourceManager.GetString("Utilities_RenameCommand_OwnerOnlyCommand", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to Success; changed username to {0}.
        /// </summary>
        public static string Utilities_RenameCommand_SuccessChangedUsernameTo0 {
            get {
                return ResourceManager.GetString("Utilities_RenameCommand_SuccessChangedUsernameTo0", resourceCulture);
            }
        }
    }
}
