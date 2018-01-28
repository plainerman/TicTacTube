namespace TicTacTubeCore.Telegram.Schedulers
{
	/// <summary>
	/// An enum that defines the different user lists.
	/// </summary>
	public enum UserList
	{
		/// <summary>
		/// All users are allowed.
		/// </summary>
		None,
		/// <summary>
		/// Blocked users are specified.
		/// </summary>
		Blacklist,
		/// <summary>
		/// Allowed users are specified.
		/// </summary>
		Whitelist
	}
}