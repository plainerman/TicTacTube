using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using TicTacTubeCore.Schedulers;

namespace TicTacTubeCore.Telegram.Schedulers
{
	/// <summary>
	/// This is a telegram bot scheduler, it hosts a telegram bot that can interact with the user.
	/// </summary>
	public abstract class TelegramBotBaseScheduler : BaseScheduler
	{
		/// <summary>
		/// The client that is used to make interactions with the telegram api.
		/// </summary>
		protected ITelegramBotClient BotClient;

		/// <summary>
		/// The text that will be sent when starting the bot.
		/// </summary>
		protected string WelcomeText = "Hello there! A warm welcome! What can I do you ask? Well — I do what I am programmed to do.";

		/// <summary>
		/// The message that will be sent, if a user is not allowed. If <c>null</c>, no message will be sent.
		/// </summary>
		protected string UserNotAllowedText = null;

		/// <summary>
		/// The user handling. If <see ref="UserList.None"/>, all users are allowd. With black / whitelist users can be selecteively allowed / forbidden. 
		/// </summary>
		public UserList UserList { get; protected set; }

		/// <summary>
		/// The users that are either allows or not allowed. 
		/// </summary>
		protected ISet<int> Users;

		/// <summary>
		/// Create a new uninitialised telegram scheduler — the <see cref="BotClient"/> has to be manually set before calling <see cref="ExecuteStart"/>.
		/// </summary>
		private TelegramBotBaseScheduler()
		{
			Users = new HashSet<int>();
		}

		/// <summary>
		/// The constructor for a new telegram scheduler. The API token can be received by chatting with @botfather. 
		/// </summary>
		/// <param name="apiToken">The api token that uniquely identifies a bot. May not be <c>null</c>.</param>
		/// <param name="userList">The user handling. If <see ref="UserList.None"/>, all users are allowd. With black / whitelist users can be selecteively allowed / forbidden. </param>
		/// <param name="proxy">The proxy that is used, if set. If <c>null</c>, no proxy will be used.</param>
		protected TelegramBotBaseScheduler(string apiToken, UserList userList = UserList.None, IWebProxy proxy = null) : this()
		{
			if (string.IsNullOrWhiteSpace(apiToken))
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(apiToken));
			if (!Enum.IsDefined(typeof(UserList), userList))
				throw new InvalidEnumArgumentException(nameof(userList), (int)userList, typeof(UserList));

			BotClient = proxy == null ? new TelegramBotClient(apiToken) : new TelegramBotClient(apiToken, proxy);
			UserList = userList;
		}

		/// <inheritdoc />
		protected override void ExecuteStart()
		{
			BotClient.StartReceiving();
			BotClient.OnMessage += OnMessageReceived;
		}

		/// <summary>
		/// This message will be executed once a bot receives a new mesasge. It checks whether the <see cref="IsMessageTypeSupported"/>, if so it will call <see cref="ProcessMessage"/>
		/// <see cref="OnUnknownMessageTypeReceived"/> otherwise.
		/// </summary>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="messageEventArgs">The messageeventarg that contain all information from the new message.</param>
		protected virtual void OnMessageReceived(object sender, MessageEventArgs messageEventArgs)
		{
			if (UserList != UserList.None)
			{
				if (UserList == UserList.Blacklist && Users.Contains(messageEventArgs.Message.From.Id) ||
					UserList == UserList.Whitelist && !Users.Contains(messageEventArgs.Message.From.Id))
				{
					if (UserNotAllowedText != null)
					{
						SendTextMessage(messageEventArgs, UserNotAllowedText);
					}
					return;
				}
			}
			if (IsMessageTypeSupported(messageEventArgs.Message.Type))
			{
				ProcessMessage(sender, messageEventArgs);
			}
			else
			{
				OnUnknownMessageTypeReceived(sender, messageEventArgs);
			}
		}

		/// <summary>
		/// Decide whether a given <paramref name="type"/> is supported or not.
		/// </summary>
		/// <param name="type">The type that will be checked.</param>
		/// <returns><c>True</c>, if the type is supported — <c>false</c> otherwise.</returns>
		protected virtual bool IsMessageTypeSupported(MessageType type) => type == MessageType.TextMessage;

		/// <summary>
		/// Once a supported message is received, this method is called.
		/// </summary>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="messageEventArgs">The messageeventarg that contain all information from the new message.</param>
		protected virtual void ProcessMessage(object sender, MessageEventArgs messageEventArgs)
		{
			if (messageEventArgs.Message.Type == MessageType.TextMessage)
			{
				if (messageEventArgs.Message.Text.StartsWith("/"))
				{
					ProcessTextCommands(messageEventArgs);
				}
				else
				{
					ProcessTextMessage(messageEventArgs);
				}
			}
		}

		/// <summary>
		/// If a newly received text message, is a normal message (i.e. no command) this method will process it.
		/// </summary>
		/// <param name="messageEventArgs">The messageeventarg that contain all information from the new message.</param>
		protected abstract void ProcessTextMessage(MessageEventArgs messageEventArgs);

		/// <summary>
		/// If a newly received text message is a command (starts with a '/') this message processes the command.
		/// </summary>
		/// <param name="messageEventArgs">The messageeventarg that contain all information from the new message.</param>
		protected virtual void ProcessTextCommands(MessageEventArgs messageEventArgs)
		{
			if (messageEventArgs.Message.Text.StartsWith("/start"))
			{
				SendTextMessage(messageEventArgs, WelcomeText);
			}
			else if (messageEventArgs.Message.Text.StartsWith("/ping"))
			{
				SendTextMessage(messageEventArgs, "pong");
			}
			else if (messageEventArgs.Message.Text.StartsWith("/say"))
			{
				SendTextMessage(messageEventArgs,
					messageEventArgs.Message.Text.Length < "/say ".Length
						? "What should I say?"
						: messageEventArgs.Message.Text.Substring("/say ".Length));
			}
			else if (messageEventArgs.Message.Text.StartsWith("/id"))
			{
				SendTextMessage(messageEventArgs, $"Your user id is: {messageEventArgs.Message.From.Id}");
			}
			else
			{
				if (!ProcessCustomCommands(messageEventArgs))
				{
					SendTextMessage(messageEventArgs, "I'm sorry, but I don't understand this command.");
				}
			}
		}

		/// <summary>
		/// This method can be used to expand the available commands. Return <c>true</c> if the command could be processed, <c>false</c> otherwise.
		/// </summary>
		/// <param name="messageEventArgs">The messageeventarg that contain all information from the new message.</param>
		/// <returns>Return <c>true</c> if the command could be processed, <c>false</c> otherwise.</returns>
		protected virtual bool ProcessCustomCommands(MessageEventArgs messageEventArgs) => false;

		/// <summary>
		/// This method can be used to easily send a text message to a chat id.
		/// </summary>
		/// <param name="chatId">The id of the chat the message is sent to.</param>
		/// <param name="message">The message that will be sent.</param>
		protected virtual void SendTextMessage(long chatId, string message)
		{
			BotClient.SendTextMessageAsync(chatId, message);
		}

		/// <summary>
		/// This method can be used to easily send a text message to a chat id.
		/// </summary>
		/// <param name="messageEventArgs">The message event args that are used to extract the chat id the message is sent to.</param>
		/// <param name="message">The message that will be sent.</param>
		protected virtual void SendTextMessage(MessageEventArgs messageEventArgs, string message)
		{
			SendTextMessage(messageEventArgs.Message.Chat.Id, message);
		}

		/// <summary>
		/// Once an unsupported message is received, this method is called.
		/// </summary>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="messageEventArgs">The messageeventarg that contain all information from the new message.</param>
		protected virtual void OnUnknownMessageTypeReceived(object sender, MessageEventArgs messageEventArgs)
		{
			SendTextMessage(messageEventArgs, "I'm sorry, but I don't support this message type.");
		}

		/// <inheritdoc />
		protected override void ExecuteStop()
		{
			BotClient.StopReceiving();
			BotClient.OnMessage -= OnMessageReceived;
		}

		/// <summary>
		/// Add a user to the user list. This list is used to decide whether an user is allowed or not (see <see cref="UserList"/>).
		/// See <see ref="Message.From.Id"/>.
		/// </summary>
		/// <param name="userId">The user id.</param>
		public void AddUser(int userId)
		{
			Users.Add(userId);
		}

		/// <summary>
		/// Remove a user from the user list. This list is used to decide whether an user is allowed or not (see <see cref="UserList"/>).
		/// </summary>
		/// <param name="userId">The user id.</param>
		public void RemoveUser(int userId)
		{
			Users.Remove(userId);
		}

		/// <summary>
		/// Check whether a user is contained in the list. This list is used to decide whether an user is allowed or not (see <see cref="UserList"/>).
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <returns><c>True</c>, if the user is contained — <c>false</c> otherwise.</returns>
		public bool ContainsUser(int userId)
		{
			return Users.Contains(userId);
		}
	}
}