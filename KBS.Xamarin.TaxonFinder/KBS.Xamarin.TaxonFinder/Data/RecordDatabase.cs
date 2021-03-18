using KBS.App.TaxonFinder.Models;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KBS.App.TaxonFinder.Data
{
    public class RecordDatabase
	{
		readonly SQLiteAsyncConnection database;

		public RecordDatabase(string dbPath)
		{
			database = new SQLiteAsyncConnection(dbPath);

			Task[] tasks = new Task[4];
			tasks[0] = database.CreateTableAsync<MediaFileModel>();
			tasks[1] = database.CreateTableAsync<RecordModel>();
			tasks[2] = database.CreateTableAsync<RegisterModel>();
			tasks[3] = database.CreateTableAsync<PositionModel>();

			Task.WhenAll(tasks);

		}
		public Task<int> Register(string deviceHash, string username)
		{
			return database.InsertOrReplaceAsync(new RegisterModel { ID = 0, DeviceHash = deviceHash, UserName = username });
		}
		public Task<RegisterModel> GetRegister()
		{
			if (database.Table<RegisterModel>().FirstOrDefaultAsync().Result != null)
			{
				if (database.Table<RegisterModel>().FirstOrDefaultAsync().Result.DeviceHash != null)
					return database.Table<RegisterModel>().FirstOrDefaultAsync();
				else
					return null;
			}
			else
			{
				return null;
			}

		}

		public string GetUserName()
		{
			if (database.Table<RegisterModel>().FirstOrDefaultAsync().Result != null)
			{
				if (database.Table<RegisterModel>().FirstOrDefaultAsync().Result.DeviceHash != null)
				{
					var reg = database.Table<RegisterModel>().FirstOrDefaultAsync();
					return reg.Result.UserName;
				}
				else
					return null;
			}
			else
			{
				return null;
			}

		}

		public Task<int> Logout()
		{
			return (database.InsertOrReplaceAsync(new RegisterModel { ID = 0, DeviceHash = null }));
		}

		public Task<List<RecordModel>> GetRecordsAsync()
		{
			return database.Table<RecordModel>().OrderByDescending(p => p.IsEditable).ThenByDescending(p => p.RecordDate).ThenByDescending(p => p.CreationDate).ToListAsync();
		}
		public void SetSynced(int localRecordId)
		{
			RecordModel record = database.Table<RecordModel>().Where(p => p.LocalRecordId == localRecordId).FirstOrDefaultAsync().Result;
			record.IsEditable = false;
			UpdateRecord(record);
		}
		public Task<RecordModel> GetRecordAsync(int localRecordId)
		{
			return database.Table<RecordModel>().Where(p => p.LocalRecordId == localRecordId).FirstOrDefaultAsync();
		}
		public Task<int> SaveRecord(RecordModel recordModel)
		{
			return (database.InsertAsync(recordModel));
		}
		public Task<int> UpdateRecord(RecordModel recordModel)
		{
			return (database.UpdateAsync(recordModel));
		}
		public Task<int> DeleteRecord(RecordModel recordModel)
		{
			return (database.DeleteAsync(recordModel));
		}

		public Task<List<MediaFileModel>> GetMediaAsync(int localRecordId)
		{
			return database.Table<MediaFileModel>().Where(p => p.LocalRecordId == localRecordId).ToListAsync();
		}
		public Task<MediaFileModel> GetMediaByPathAsync(string mediaPath)
		{
			return database.Table<MediaFileModel>().Where(p => p.Path == mediaPath).FirstOrDefaultAsync();
		}
		public Task<int> SaveMedia(MediaFileModel mediaFileModel)
		{
			return (database.InsertAsync(mediaFileModel));
		}
		public Task<int> DeleteMedia(MediaFileModel mediaFileModel)
		{
			return (database.DeleteAsync(mediaFileModel));
		}

		public Task<int> SavePosition(PositionModel positionModel)
		{
			return (database.InsertAsync(positionModel));
		}
		public Task<int> DeletePosition(PositionModel positionModel)
		{
			return (database.DeleteAsync(positionModel));

		}
		public Task<List<PositionModel>> GetPositionAsync(int localRecordId)
		{
			return database.Table<PositionModel>().Where(p => p.LocalRecordId == localRecordId).ToListAsync();
		}
		public Task<List<PositionModel>> GetAllPositionsAsync()
		{
			return database.Table<PositionModel>().ToListAsync();
		}
	}
}
