using KBS.App.TaxonFinder.Models;
using System;
using System.Threading.Tasks;

namespace KBS.App.TaxonFinder.Services
{
    /// <summary>
    /// Provides methods to communicate with the API.
    /// </summary>
    public interface IMobileApi
	{

		/// <summary>
		/// Logs a user in by sending username, password and deviceId to service.
		/// </summary>
		/// <param name="username">The username of the user to login.</param>
		/// <param name="password">The password of the user to login</param>
		/// <returns>Returns deviceHash of logged in user or error message.</returns>
		Task<string> Register(string username, string password);

		/// <summary>
		/// Registers a user to serverside by sending userdata to the service.
		/// </summary>
		/// <param name="givenname">The givenname of the user to register.</param>
		/// <param name="surname">The surname of the user to register.</param>
		/// <param name="username">The username of the user to register.</param>
		/// <param name="password">The password of the user to register.</param>
		/// <param name="comment">The comment of the user to register.</param>
		/// <param name="source">The source of the user to register.</param>
		/// <returns>Returns content from request to service.</returns>
		Task<string> AddNewUser(string givenname, string surname, string username, string password, string comment, string source);

		/// <summary>
		/// Saves an Advice by sending an AdviceJsonItem to the service.
		/// </summary>
		/// <param name="adviceJsonItem">The AdviceJsonItem to save.</param>
		/// <returns>Returns "true"-string if the request was successfull.</returns>
		Task<string> SaveAdvicesByDevice(AdviceJsonItem[] adviceJsonItem);

		/// <summary>
		/// Requests the changes made to the Advices of the user.
		/// </summary>
		/// <param name="auth">AuthorizationJson of the user.</param>
		/// <returns>Returns an AdviceJsonItem-string with the changed Advices.</returns>
		Task<string> GetChangesByDevice(AuthorizationJson auth);

		/// <summary>
		/// Sends feedback to service.
		/// </summary>
		/// <param name="text">The feedback text to send.</param>
		/// <param name="mail">The optional e-mail adress of the user.</param>
		/// <returns>Returns a "success"-string if the request was successfull.</returns>
		Task<string> SendFeedback(string text, string mail);
	}

	public class MobileRegisterEventArgs : EventArgs
	{
		public string DeviceHash { get; set; }
		public bool Success { get; set; }
	}
}
