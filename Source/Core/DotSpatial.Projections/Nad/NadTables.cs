// ********************************************************************************************************
// Product Name: DotSpatial.Projection
// Description:  The basic module for MapWindow version 6.0
// ********************************************************************************************************
//
// The Original Code is from MapWindow.dll version 6.0
//
// The Initial Developer of this Original Code is Ted Dunsford. Created 8/17/2009 4:52:04 PM
//
// Contributor(s): (Open source contributors should list themselves and their modifications here).
//        Name         |    Date    |        Comment
// --------------------|------------|------------------------------------------------------------
// Ted Dunsford        |   5/3/2010 |  Updated project to DotSpatial.Projection and license to LGPL
// ********************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace DotSpatial.Projections.Nad
{
    public class NadTables
    {
        #region Private Variables

        private readonly Dictionary<string, Lazy<NadTable>> _tables = new();

        #endregion

        #region Methods

        public void InitializeEmbeddedGrids()
        {
            var a = Assembly.GetExecutingAssembly();
            foreach (var s in a.GetManifestResourceNames())
            {
                string[] ss = s.Split('.');
                if (ss.Length < 3)
                    continue;
                string coreName = "@" + ss[ss.Length - 3];
                if (_tables.ContainsKey(coreName))
                    continue;
                string ext = Path.GetExtension(s).ToLower();
                if (ext != ".lla" && ext != ".dat" && ext != ".gsb")
                    continue;
                if (ss[ss.Length - 2] != "ds")
                    continue; // only encoded by DeflateStreamUtility
                using (var str = a.GetManifestResourceStream(s))
                {
                    if (str == null)
                        continue;
                }
                var s1 = s;
                _tables.Add(coreName, new Lazy<NadTable>(() => NadTable.FromSourceName(s1, true, true), true));
            }
        }

        /// <summary>
        /// Load grids from the default GeogTransformsGrids subfolder
        /// </summary>
        public void InitializeExternalGrids()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            string strGridsFolder = Path.Combine(Path.GetDirectoryName(a.Location), "GeogTransformGrids");
            InitializeExternalGrids(strGridsFolder, true);
        }

        /// <summary>
        /// Load grids from the specified folder
        /// </summary>
        public void InitializeExternalGrids(string strGridsFolder, bool recursive)
        {
            SearchOption opt = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            foreach (var gridType in new[] { "*.los", "*.gsb", "*.dat", "*.lla" })
                foreach (var s in Directory.EnumerateFiles(strGridsFolder, gridType, opt))
                {
                    string coreName = "@" + Path.GetFileNameWithoutExtension(s);
                    if (_tables.ContainsKey(coreName)) continue;
                    string ext = Path.GetExtension(s).ToLower();
                    if (ext == ".los" && !File.Exists(s.Replace(".los", ".las"))) continue;
                    var s1 = s;
                    _tables.Add(coreName, new Lazy<NadTable>(() => NadTable.FromSourceName(s1, false), true));
                }
        }

        public void InitializeExternalGrids(IEnumerable<(string FileName, Func<Stream> StreamFactory)> grids)
        {
            var gridSets = grids.GroupBy(x => "@" + Path.GetFileNameWithoutExtension(x.FileName));
            foreach (var gridGroup in gridSets)
            {
                if (_tables.ContainsKey(gridGroup.Key)) continue;

                var exts = gridGroup.Select(x => Path.GetExtension(x.FileName).ToLower()).ToArray();
                if (exts.Contains(".los") && !exts.Contains(".las")) continue;

                foreach (var grid in gridGroup)
                {
                    // Capture for labmda
                    var streamFactory = grid.StreamFactory;
                    var coreName = gridGroup.Key;
                    var ext = Path.GetExtension(grid.FileName).ToLower();

                    _tables.Add(gridGroup.Key, ext switch
                    {
                        ".los" => new Lazy<NadTable>(() => new LasLosNadTable(streamFactory)),
                        ".lla" => new Lazy<NadTable>(() => new LlaNadTable(streamFactory)),
                        ".gsb" => new Lazy<NadTable>(() => new GsbNadTable(streamFactory)),
                        ".dat" => new Lazy<NadTable>(() => new DatNadTable(streamFactory)),
                        _ => throw new ArgumentException($"Unsuppored grid type {ext}", "grids")
                    });
                }

            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets an array of the lla tables that have been added as a resource
        /// </summary>
        public Dictionary<string, Lazy<NadTable>> Tables
        {
            get { return _tables; }
        }

        #endregion
    }
}