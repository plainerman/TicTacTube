using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using log4net;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TicTacTubeCore.Schedulers;

namespace TicTacTubeCore.Telegram.Schedulers
{
	/// <summary>
	///     This is a telegram bot scheduler, it hosts a telegram bot that can interact with the user.
	/// </summary>
	public abstract class TelegramBotBaseScheduler : BaseScheduler
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(TelegramBotBaseScheduler));

		/// <summary>
		/// The default constructor of this class, tries to fetch the telegram api-token from a system variable. This variable
		/// contains the key to this system variable.
		/// </summary>
		public const string TelegramSystemVariable = "TELEGRAM_TOKEN";

		/// <summary>
		///     The client that is used to make interactions with the telegram api.
		/// </summary>
		protected ITelegramBotClient BotClient;

		/// <summary>
		///     The message that will be sent, if a user is not allowed. If <c>null</c>, no message will be sent.
		/// </summary>
		protected string UserNotAllowedText = null;

		/// <summary>
		///     The users that are either allows or not allowed.
		/// </summary>
		protected ISet<int> Users;

		/// <summary>
		///     The text that will be sent when starting the bot.
		/// </summary>
		protected string WelcomeText =
			"Hello there! A warm welcome! What can I do you ask? Well — I do what I am programmed to do.";

		/// <summary>
		///     The user handling. If <see ref="UserList.None" />, all users are allowd. With black / whitelist users can be
		///     selecteively allowed / forbidden.
		/// </summary>
		public UserList UserList { get; protected set; }

		/// <summary>
		///     Whether bots are allowed or not.
		/// </summary>
		public bool AllowBots { get; set; } = false;

		/// <inheritdoc />
		/// <summary>
		///     Create a new uninitialised telegram scheduler — the <see cref="F:TicTacTubeCore.Telegram.Schedulers.TelegramBotBaseScheduler.BotClient" /> has to be manually set before calling
		///     <see cref="M:TicTacTubeCore.Telegram.Schedulers.TelegramBotBaseScheduler.ExecuteStart" />.
		/// </summary>
		private TelegramBotBaseScheduler()
		{
			Users = new HashSet<int>();
		}

		/// <inheritdoc />
		/// <summary>
		///     The constructor for a new telegram scheduler. The API token can be received by chatting with @botfather.
		/// </summary>
		/// <param name="apiToken">The api token that uniquely identifies a bot. If this api-token is <code>null</code> (or only whitespace), the constructor will try to
		/// fetch the api-token from a systemvariable identified by <see cref="TelegramSystemVariable"/>.</param>
		/// <param name="userList">
		///     The user handling. If <see ref="UserList.None" />, all users are allowd. With black / whitelist
		///     users can be selecteively allowed / forbidden.
		/// </param>
		/// <param name="proxy">The proxy that is used, if set. If <c>null</c>, no proxy will be used.</param>
		protected TelegramBotBaseScheduler(string apiToken = null, UserList userList = UserList.None, IWebProxy proxy = null) :
			this()
		{
			if (string.IsNullOrWhiteSpace(apiToken))
				apiToken = Environment.GetEnvironmentVariable(TelegramSystemVariable);

			if (string.IsNullOrWhiteSpace(apiToken))
				throw new ArgumentException($"Value cannot be null or whitespace. System variable ${TelegramSystemVariable} is also not set.", nameof(apiToken));
			if (!Enum.IsDefined(typeof(UserList), userList))
				throw new InvalidEnumArgumentException(nameof(userList), (int) userList, typeof(UserList));

			BotClient = proxy == null ? new TelegramBotClient(apiToken) : new TelegramBotClient(apiToken, proxy);
			UserList = userList;

			if (UserList != UserList.None)
				Log.Info($"User list specified to {userList.ToString()}");
		}

		/// <inheritdoc />
		protected override void ExecuteStart()
		{
			BotClient.StartReceiving();
			BotClient.OnMessage += OnMessageReceived;

			Log.Info("Telegram bot is now running...");
		}

		/// <summary>
		///     This message will be executed once a bot receives a new mesasge. It checks whether the
		///     <see cref="IsMessageTypeSupported" />, if so it will call <see cref="ProcessMessage" />
		///     <see cref="OnUnknownMessageTypeReceived" /> otherwise.
		/// </summary>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="messageEventArgs">The messageeventarg that contain all information from the new message.</param>
		protected virtual void OnMessageReceived(object sender, MessageEventArgs messageEventArgs)
		{
			if (!AllowBots && messageEventArgs.Message.From.IsBot)
			{
				Log.Warn(
					$"Unauthorized bot tried to access: {messageEventArgs.Message.From.Id}, {messageEventArgs.Message.From.Username}, {messageEventArgs.Message.From.FirstName}, {messageEventArgs.Message.From.LastName}, language={messageEventArgs.Message.From.LanguageCode}, isBot={messageEventArgs.Message.From.IsBot}\n{messageEventArgs.Message.Type}: {messageEventArgs.Message.Text}");

				if (UserNotAllowedText != null)
					SendTextMessage(messageEventArgs.Message, UserNotAllowedText);
			}

			if (UserList != UserList.None)
				if (UserList == UserList.Blacklist && Users.Contains(messageEventArgs.Message.From.Id) ||
				    UserList == UserList.Whitelist && !Users.Contains(messageEventArgs.Message.From.Id))
				{
					if (UserNotAllowedText != null)
						SendTextMessage(messageEventArgs.Message, UserNotAllowedText);

					Log.Warn(
						$"Unauthorized user tried to access: {messageEventArgs.Message.From.Id}, {messageEventArgs.Message.From.Username}, {messageEventArgs.Message.From.FirstName}, {messageEventArgs.Message.From.LastName}, language={messageEventArgs.Message.From.LanguageCode}, isBot={messageEventArgs.Message.From.IsBot}\n{messageEventArgs.Message.Type}: {messageEventArgs.Message.Text}");

					return;
				}

			if (IsMessageTypeSupported(messageEventArgs.Message.Type))
				ProcessMessage(messageEventArgs.Message);
			else
				OnUnknownMessageTypeReceived(messageEventArgs.Message);
		}

		/// <summary>
		///     Decide whether a given <paramref name="type" /> is supported or not.
		/// </summary>
		/// <param name="type">The type that will be checked.</param>
		/// <returns><c>True</c>, if the type is supported — <c>false</c> otherwise.</returns>
		protected virtual bool IsMessageTypeSupported(MessageType type) => type == MessageType.TextMessage;

		/// <summary>
		///     Once a supported message is received, this method is called.
		/// </summary>
		/// <param name="message">The message that contains all information from the new message.</param>
		protected virtual void ProcessMessage(Message message)
		{
			if (message.Type != MessageType.TextMessage) return;

			if (message.Text.StartsWith("/"))
				ProcessTextCommands(message);
			else
				ProcessTextMessage(message);
		}

		/// <summary>
		///     If a newly received text message, is a normal message (i.e. no command) this method will process it.
		/// </summary>
		/// <param name="message">The message that contains all information from the new message.</param>
		protected abstract void ProcessTextMessage(Message message);

		/// <summary>
		///     If a newly received text message is a command (starts with a '/') this message processes the command.
		/// </summary>
		/// <param name="message">The message that contain all information from the new message.</param>
		protected virtual void ProcessTextCommands(Message message)
		{
			if (message.Text.StartsWith("/start"))
			{
				SendTextMessage(message, WelcomeText);
				Log.Info(
					$"New user! {message.From.Id}, {message.From.Username}, {message.From.FirstName}, {message.From.LastName}, language={message.From.LanguageCode}, isBot={message.From.IsBot}");
			}
			else if (message.Text.StartsWith("/ping"))
			{
				SendTextMessage(message, "pong");
			}
			else if (message.Text.StartsWith("/say"))
			{
				SendTextMessage(message,
					message.Text.Length < "/say ".Length
						? "What should I say?"
						: message.Text.Substring("/say ".Length));
			}
			else if (message.Text.StartsWith("/id"))
			{
				SendTextMessage(message, $"Your user id is: {message.From.Id}");
			}
			else
			{
				if (!ProcessCustomCommands(message))
				{
					SendTextMessage(message, "I'm sorry, but I don't understand this command.");
					Log.Info($"An unknown command has been received {message.Text}.");
				}
			}
		}

		/// <summary>
		///     This method can be used to expand the available commands. Return <c>true</c> if the command could be processed,
		///     <c>false</c> otherwise.
		/// </summary>
		/// <param name="message">The messageeventarg that contains all information from the new message.</param>
		/// <returns><c>True</c> if the command could be processed, <c>false</c> otherwise.</returns>
		protected virtual bool ProcessCustomCommands(Message message) => false;

		/// <summary>
		///     This method can be used to easily send a text message to a chat id.
		/// </summary>
		/// <param name="chatId">The id of the chat the message is sent to.</param>
		/// <param name="message">The message that will be sent.</param>
		protected virtual void SendTextMessage(long chatId, string message)
		{
			BotClient.SendTextMessageAsync(chatId, message);
		}

		/// <summary>
		///     This method can be used to easily send a text message to a chat id.
		/// </summary>
		/// <param name="message">The message that is used to extract the chat id the <paramref name="messageText" /> is sent to.</param>
		/// <param name="messageText">The message that will be sent.</param>
		protected virtual void SendTextMessage(Message message, string messageText)
		{
			SendTextMessage(message.Chat.Id, messageText);
		}

		/// <summary>
		///     Once an unsupported message is received, this method is called.
		/// </summary>
		/// <param name="message">The message that contains all information from the new message.</param>
		protected virtual void OnUnknownMessageTypeReceived(Message message)
		{
			SendTextMessage(message, "I'm sorry, but I don't support this message type.");
			Log.Debug($"An unknown message type has been received {message.Type}.");
		}

		/// <inheritdoc />
		protected override void ExecuteStop()
		{
			BotClient.StopReceiving();
			BotClient.OnMessage -= OnMessageReceived;

			Log.Warn("Telegram bot has been stopped...");
		}

		/// <summary>
		///     Add a user to the user list. This list is used to decide whether an user is allowed or not (see
		///     <see cref="UserList" />).
		///     See <see ref="Message.From.Id" />.
		/// </summary>
		/// <param name="userId">The user id.</param>
		public void AddUser(int userId)
		{
			Users.Add(userId);
		}

		/// <summary>
		///     Remove a user from the user list. This list is used to decide whether an user is allowed or not (see
		///     <see cref="UserList" />).
		/// </summary>
		/// <param name="userId">The user id.</param>
		public void RemoveUser(int userId)
		{
			Users.Remove(userId);
		}

		/// <summary>
		///     Check whether a user is contained in the list. This list is used to decide whether an user is allowed or not (see
		///     <see cref="UserList" />).
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <returns><c>True</c>, if the user is contained — <c>false</c> otherwise.</returns>
		public bool ContainsUser(int userId) => Users.Contains(userId);
	}
}