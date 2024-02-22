namespace DobirnaGraServer.Assets
{
	public class ScopeFile : IAsyncDisposable, IDisposable
	{
		public struct ContentMetadata
		{
			public string ContentType { get; set; }
		}

		public delegate void EventFileDeleted(long id);

		public event EventFileDeleted? OnFileDeleted;

		private readonly FileStream _file;

		public string PathFile => _file.Name;

		public long Id { get; init; }

		public ScopeFile()
		{
			var path = Path.GetTempFileName();
			_file = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
			Id = BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0);
		}

		public ContentMetadata Metadata { get; set; }

		public async Task CopyFromAsync(Stream source)
		{
			if(_file.CanSeek) 
				_file.Seek(0, SeekOrigin.Begin);

			await source.CopyToAsync(_file);

			await _file.FlushAsync();
		}

		public async Task<byte[]> GetAllBytesAsync(CancellationToken cancellationToken)
		{
			using var memoryStream = new MemoryStream();

			if (_file.CanSeek)
				_file.Seek(0, SeekOrigin.Begin);

			await _file.CopyToAsync(memoryStream, cancellationToken);

			return memoryStream.ToArray();
		}

		public async ValueTask DisposeAsync()
		{
			var path = _file.Name;

			await _file.DisposeAsync();
			File.Delete(path);

			OnFileDeleted?.Invoke(Id);
		}

		public void Dispose()
		{
			var path = _file.Name;

			_file.Dispose();
			File.Delete(path);

			OnFileDeleted?.Invoke(Id);
		}
	}
}
