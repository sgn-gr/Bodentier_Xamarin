using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KBS.App.TaxonFinder.Models
{
	public class AuthorizationJson
	{
		[DataMember]
		public string DeviceId { get; set; }
		[DataMember]
		public string DeviceHash { get; set; }
		public AuthorizationJson(string deviceId, string deviceHash)
		{
			DeviceId = deviceId;
			DeviceHash = deviceHash;
		}
	}
}
