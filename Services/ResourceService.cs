using DobirnaGraServer.Assets;

namespace DobirnaGraServer.Services
{
	public class ResourceService
	{
		private Dictionary<long, ScopeFile> _cacheUserImage = new();

		private object _lock = new();

		public int NumberImages => _cacheUserImage.Count;

		public async Task<ScopeFile> SaveImageAsync(Stream file, string contentType)
		{
			ScopeFile scopeFile = new();

			scopeFile.OnFileDeleted += OnFileDeleted;

			lock (_lock)
			{
				_cacheUserImage.Add(scopeFile.Id, scopeFile);
			}

			scopeFile.Metadata = new ScopeFile.ContentMetadata() { ContentType = contentType };

			await scopeFile.CopyFromAsync(file);

			return scopeFile;
		}

		public async Task DeleteFileAsync(long id)
		{
			if (_cacheUserImage.TryGetValue(id, out ScopeFile? file))
			{
				await file.DisposeAsync();
			}
		}

		public bool TryGetFile(long id, out ScopeFile? file)
		{
			return _cacheUserImage.TryGetValue(id, out file);
		}

		private void OnFileDeleted(long id)
		{
			lock (_lock)
			{
				_cacheUserImage.Remove(id);
			}
		}
	}
}
