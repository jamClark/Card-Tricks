using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using CardTricks.Attributes;
using System.ServiceModel;

namespace CardTricks.Models
{
    /// <summary>
    /// Container of a Game project and all related
    /// information for storing, accessing, and editing it.
    /// </summary>
    //[KnownType(typeof(AbstractTestObj))]
    [SaveableObject]
    [DataContract]
    public class Workspace
    {
        #region Private Members
        [Saveable(Name="Version")]
        [DataMember(Name = "Version", Order = 0)]
        private string _Version = "0.1b"; //don't serialize this, we do it with the public property
        #endregion


        #region Public Properties
        [Saveable]
        [DataMember(Order = 1)]
        public string ProjectName { get; private set; }
        [Saveable]
        [DataMember(Order = 2)]
        public Game Game { get; private set; }
        
        //non-serialized data
        public string Version { get { return _Version; } private set { _Version = value; } }
        [DataMember(Order = 3)]
        public string RootFolder { get; private set; } //NOTE: We'll need to set this when deserializing the file
        public string ImageFolder { get { return Path.Combine(RootFolder, "Images"); } }
        public string ExportsFolder { get { return Path.Combine(RootFolder, "Exports"); } }
        #endregion


        #region Public Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="gameName"></param>
        public Workspace(string projectName, string gameName, string rootFolder)
        {
            if (gameName == null) throw new InvalidDataException("No name was given to the new game.");
            if (projectName == null) throw new InvalidDataException("No name was given to the new project.");
            if (rootFolder == null) throw new InvalidDataException("No workspace directory was supplied to the new project.");
            if (!Directory.Exists(rootFolder)) throw new InvalidDataException("The directory '" + rootFolder + "' does not exist.");

            Game = new Game(gameName);
            
            ProjectName = projectName;
            RootFolder = rootFolder;

        }

        /// <summary>
        /// This is primarliy used when loading a workspace as the saved RootFolder will be
        /// tied to the system on which the file was originally saved. With this we can
        /// set a new root directory if necessary.
        /// </summary>
        /// <param name="dir"></param>
        public void ProvideWorkspaceRoot(string dir)
        {
            RootFolder = dir;
        }
        #endregion
    }
}
