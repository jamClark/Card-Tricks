using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace Sluggy.Utility.AssetManager
	{
	/// <summary>
	/// Internal structure of an asset stored within an asset manager.
	/// </summary>
	/// <typeparam name="type"></typeparam>
	public class AssetObject<type> : IDisposable
		{
		public type Data;
		public int RefCount = 0;
		private bool Disposed = false;
		
		~AssetObject()
			{
			Dispose(false);
			}
		
		public void Dispose()
			{
			Dispose(true);
			GC.SuppressFinalize(this);
			}
				
		protected virtual void Dispose(bool disposing)
			{
			if(!Disposed)
				{
				if(disposing)
					{
					//unmanaged
					if(Data is IDisposable)
						{
						IDisposable temp = (IDisposable)Data;
						temp.Dispose();
						}
					}
				//managed
				}			
			Disposed = true;
			}
		}
	
	/// <summary>
	/// Manages loaded files by ensuring that no two copies of the same file from the same directory
	/// are loaded. It allows all assets derived from IAsset to use RAII-techniques without making redudant
	/// copies of the same data. When an asset tries to access its data it actualy references a lookup within
	/// its associated asset manager. An internal reference count within the manager ensures that when all
	/// copies have relenquished their data using 'Dispose' it can safely dipsose of the original master-copy.
	/// </summary>
	/// <typeparam name="type"></typeparam>
	public abstract class AssetManager<type> : IDisposable
		{
		private Dictionary<string,AssetObject<type>> AssetMap = new Dictionary<string,AssetObject<type>>();
		private bool Disposed;
		
		~AssetManager()
			{
			Dispose(false);
			}
		
		internal void RegisterAsset(string fileName)
			{
			if(AssetMap.ContainsKey(fileName))
				{
				AssetMap[fileName].RefCount += 1;
				}
			else{
				AssetMap.Add(fileName,LoadAsset(fileName));
				}
			}
		
		internal void RegisterAsset(string fileName,params object[] extra)
			{
			if(AssetMap.ContainsKey(fileName))
				{
				AssetMap[fileName].RefCount += 1;
				}
			else{
				AssetMap.Add(fileName,LoadAsset(fileName,extra));
				}
			}
		
		internal void UnregisterAsset(string fileName)
			{
			if(AssetMap.ContainsKey(fileName))
				{
				AssetObject<type> asset;
				
				asset = AssetMap[fileName];
				asset.RefCount -= 1;
				if(asset.RefCount <= 0)
					{
					DisposeAsset(fileName);
					}
				}
			}
		
		internal type AccessAsset(string fileName)
			{
			return AssetMap[fileName].Data;
			}
		
		private void DisposeAsset(string fileName)
			{
			AssetMap[fileName].Dispose();
			AssetMap.Remove(fileName);
			}
		
		internal abstract AssetObject<type> LoadAsset(string fileName,params object[] extra);
		
		public void Dispose()
			{
			Dispose(true);
			GC.SuppressFinalize(this);
			}
		
		protected virtual void Dispose(bool disposing)
			{
			if(!Disposed)
				{
				if(disposing)
					{
					//unmanaged resources
					foreach(AssetObject<type> asset in AssetMap.Values)
						{
						asset.Dispose();
						}
					}
				//managed resources
				}
			
			Disposed = true;
			}
		}
	
	/// <summary>
	/// An instanced copy of an asset loaded from a file. The original data is actually contained
	/// within the associated AssetManager. When done with the data, call 'Dispose' to ensure that
	/// the asset manager has an accurate reference count and can properly release the original
	/// master-copy when it is no longer in use.
	/// </summary>
	/// <typeparam name="type"></typeparam>
	public class Asset<type> : IDisposable
		{
		private AssetManager<type> refAssetManager;
		private string FileName;
		private bool Disposed = false;
		
		public Asset(AssetManager<type> manager,string fileName)
			{
			System.Diagnostics.Debug.Assert(manager != null && fileName != null && fileName.Length > 0);
			
			refAssetManager = manager;
			FileName = fileName;		
			manager.RegisterAsset(fileName);
			}
					
		public Asset(AssetManager<type> manager,string fileName,params object[] extra)
			{
			System.Diagnostics.Debug.Assert(manager != null && fileName != null && fileName.Length > 0);
			
			refAssetManager = manager;
			FileName = fileName;		
			manager.RegisterAsset(fileName,extra);
			}
		
		~Asset()
			{
			Dispose(false);
			}
		
		public void Dispose()
			{
			Dispose(true);
			GC.SuppressFinalize(this);
			}
		
		protected virtual void Dispose(bool isDisposing)
			{
			if(!Disposed)
				{
				if(isDisposing)
					{
					//unmanged resources
					refAssetManager.UnregisterAsset(FileName);
					}
				//managed resources
				}
			
			Disposed = true;
			}
		
		public type Data
			{
			get {
				return refAssetManager.AccessAsset(FileName);
				}
			}
		}
		
	}


