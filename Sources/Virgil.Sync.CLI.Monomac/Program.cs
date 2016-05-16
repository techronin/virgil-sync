﻿using Virgil.CLI.Common;
using System;
using MonoMac.Security;
using MonoMac.Foundation;

namespace Virgil.Sync.CLI.Monomac
{
	using CommandLine;
	using Virgil.CLI.Common.Handlers;
	using Virgil.CLI.Common.Options;

	class MainClass
	{
		public static int Main(string[] args)
		{
			var parserResult = Parser.Default.ParseArguments<ConfigureOptions, StartOptions>(args);

			var configHandler = new ConfigHandler();
			var startHandler = new StartHandler();

			return parserResult.MapResult(
				(ConfigureOptions options) => configHandler.Handle(options),
				(StartOptions options) => startHandler.Handle(options),
				errs => 1);
		}
	}

	public static class KeychainAccess
	{
		// Update to the name of your service
		private const string ServiceName = "Virgil.Sync.CLI.Monomac";

		public static bool GetPassword(string username, out string password)
		{
			SecRecord searchRecord;
			var record = FetchRecord(username, out searchRecord);

			if (record == null)
			{
				password = string.Empty;
				return false;
			}

			password = NSString.FromData(record.ValueData, NSStringEncoding.UTF8);
			return true;
		}
			
		public static void SetPassword(string username, string password)
		{
			SecRecord searchRecord;
			var record = FetchRecord(username, out searchRecord);

			if (record == null)
			{
				record = new SecRecord(SecKind.InternetPassword)
				{
					Service = ServiceName,
					Label = ServiceName,
					Account = username,
					ValueData = NSData.FromString(password)
				};

				SecKeyChain.Add(record);
				return;
			}

			record.ValueData = NSData.FromString(password);
			SecKeyChain.Update(searchRecord, record);
		}
			
		public static void ClearPassword(string username)
		{
			var searchRecord = new SecRecord(SecKind.InternetPassword)
			{
				Service = ServiceName,
				Account = username
			};

			SecKeyChain.Remove(searchRecord);
		}
			
		private static SecRecord FetchRecord(string username, out SecRecord searchRecord)
		{
			searchRecord = new SecRecord(SecKind.InternetPassword)
			{
				Service = ServiceName,
				Account = username
			};

			SecStatusCode code;
			var data = SecKeyChain.QueryAsRecord(searchRecord, out code);

			if (code == SecStatusCode.Success)
				return data;
			else
				return null;
		}
	}
}
