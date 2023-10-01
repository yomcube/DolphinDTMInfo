// https://tasvideos.org/EmulatorResources/Dolphin/DTM

using System;
using System.Collections;
using System.IO;
using System.Text;

namespace DolphinDTMInfo {

	public static class Program
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("DolphinDTMInfo\n==============");
			if (args.Length == 0)
			{
				Console.WriteLine("Please specify a valid DTM file.");
				return;
			}
			FileInfo fi = new FileInfo(args[0]);
			if (!fi.Exists)
			{
				Console.WriteLine(FileError(fi.Name, "does not exist."));
				return;
			}

			byte[] bytes = null;

			using (FileStream stream = fi.OpenRead()) {
				bytes = new byte[stream.Length];
				stream.Read(bytes, 0, (int)stream.Length);
			}

			// Signature: 44 4D 54 1A
			bool isValidDtm = bytes[0] == 0x44 && bytes[1] == 0x54 && bytes[2] == 0x4D && bytes[3] == 0x1A;

			if (!isValidDtm)
			{
				Console.WriteLine(FileError(fi.Name, "is not a valid DTM."));
			}

			// Game ID
			string gameId = Encoding.UTF8.GetString(bytes, 0x04, 6);
			Console.WriteLine("Game ID: " + gameId);

			// Is Wii game
			bool isWii = bytes[0x0A] != 0;
			Console.WriteLine("Is Wii game: " + isWii);

			// Controllers
			string controllers = "";
			BitArray cBits = new BitArray(new byte[] { bytes[0x0B] });
			if (cBits[0]) controllers += "GC1 ";
			if (cBits[1]) controllers += "GC2 ";
			if (cBits[2]) controllers += "GC3 ";
			if (cBits[3]) controllers += "GC4 ";
			if (cBits[4]) controllers += "Wii1 ";
			if (cBits[5]) controllers += "Wii2 ";
			if (cBits[6]) controllers += "Wii3 ";
			if (cBits[7]) controllers += "Wii4 ";
			Console.WriteLine("Controllers: " + (controllers == "" ? "None" : controllers));

			// Starts from savestate
			bool startsFromSavestate = bytes[0x0C] != 0;
			Console.WriteLine("Starts from savestate: " + startsFromSavestate);

			// VI count
			long viCount = BitConverter.ToInt64(bytes, 0x0D);
			Console.WriteLine("VI count: " + viCount);

			// Input count
			long inputCount = BitConverter.ToInt64(bytes, 0x15);
			Console.WriteLine("Input count: " + inputCount);

			// Lag count
			long lagCount = BitConverter.ToInt64(bytes, 0x1D);
			Console.WriteLine("Lag count: " + lagCount);

			// Rerecord count
			int rerecords = BitConverter.ToInt32(bytes, 0x2D);
			Console.WriteLine("Rerecords: " + rerecords);

			// Author
			string author = Encoding.UTF8.GetString(bytes, 0x31, 32);
			Console.WriteLine("Author: " + author);

			// Video backend
			string videoBackend = Encoding.UTF8.GetString(bytes, 0x51, 16);
			Console.WriteLine("Video backend: " + videoBackend);

			// Audio emulator
			string audioEmulator = Encoding.UTF8.GetString(bytes, 0x61, 16);
			Console.WriteLine("Audio backend: " + audioEmulator);

			// MD5 hash
			byte[] md5 = new byte[16];
			Array.Copy(bytes, 0x71, md5, 0, 16);
			string md5str = "";
			foreach (byte b in md5) md5str += b.ToString("X2");
			Console.WriteLine("MD5 hash of game: " + md5str);

			// Start time
			long startTime = BitConverter.ToInt64(bytes, 0x81);
			DateTime startTimeDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(startTime);
			Console.WriteLine("Start time (UNIX): " + startTime);
			Console.WriteLine("Start time (UTC): " + startTimeDateTime);
			Console.WriteLine("Start time (local): " + startTimeDateTime.ToLocalTime());

			// Saved config valid
			bool savedConfigValid = bytes[0x89] != 0;
			Console.WriteLine("Saved config is valid: " + savedConfigValid);

			// Idle skipping
			bool idleSkipping = bytes[0x8A] != 0;
			Console.WriteLine("Idle skipping on: " + idleSkipping);

			// Dual core enabled
			bool dualCore = bytes[0x8B] != 0;
			Console.WriteLine("Dual core enabled: " + dualCore);

			// Progressive scan enabled
			bool progressiveScan = bytes[0x8C] != 0;
			Console.WriteLine("Progressive scan enabled: " + progressiveScan);

			// DSP HLE enabled
			bool dspHle = bytes[0x8D] != 0;
			Console.WriteLine("DSP HLE enabled: " + dspHle + " (" + (dspHle ? "HLE" : "LLE") + ")");

			// Fast disc speed
			bool fastDisc = bytes[0x8E] != 0;
			Console.WriteLine("Fast disc speed enabled: " + fastDisc);

			// CPU
			byte cpu = bytes[0x8F];
			string cpuName = cpu == 0 ? "interpreter" : (cpu == 1 ? "JIT" : "JITIL");
			Console.WriteLine("CPU core: " + cpu + " (" + cpuName + ")");

			// EFB access
			bool efbAccess = bytes[0x90] != 0;
			Console.WriteLine("EFB access enabled: " + efbAccess);

			// EFB copy
			bool efbCopy = bytes[0x91] != 0;
			Console.WriteLine("EFB copy enabled: " + efbCopy);

			// EFB to texture
			bool efbTexture = bytes[0x92] != 0;
			Console.WriteLine("Copy EFB to texture: " + efbTexture + " (" + (efbTexture ? "texture" : "ram") + ")");

			// EFB copy cache
			bool efbCache = bytes[0x93] != 0;
			Console.WriteLine("EFB copy cache: " + efbCache);

			// Emulate format changes
			bool emuFormatChanges = bytes[0x94] != 0;
			Console.WriteLine("Emulate format changes: " + emuFormatChanges);

			// Use XFB emulation
			bool xfbEmu = bytes[0x95] != 0;
			Console.WriteLine("Use XFB emulation: " + xfbEmu);

			// Use real XFB emulation
			bool xfbReal = bytes[0x96] != 0;
			Console.WriteLine("Use real XFB emulation: " + xfbReal);

			// Memcards present
			bool memA = bytes[0x97] == 1 || bytes[0x97] == 3;
			bool memB = bytes[0x97] == 2 || bytes[0x97] == 3;
			string memcards = (memA ? "A " : "") + (memB ? "B" : "");
			Console.WriteLine("Memory cards present: " + (memcards == "" ? "None" : memcards));

			// Memcard blank
			bool memcardBlank = bytes[0x98] != 0;
			Console.WriteLine("Memory card blank: " + memcardBlank);

			// Bongos plugged
			string bongos = "";
			BitArray bBits = new BitArray(new byte[] { bytes[0x99] });
			if (bBits[0]) bongos += "1 ";
			if (bBits[1]) bongos += "2 ";
			if (bBits[2]) bongos += "3 ";
			if (bBits[3]) bongos += "4";
			Console.WriteLine("Bongos plugged in: " + (bongos == "" ? "None" : bongos));

			// Sync GPU thread
			bool gpuSync = bytes[0x9A] != 0;
			Console.WriteLine("Sync GPU thread: " + gpuSync);

			// Recorded in netplay
			bool netplay = bytes[0x9B] != 0;
			Console.WriteLine("Recorded in a netplay session: " + netplay);

			// SYSCONF PAL60
			bool pal60 = bytes[0x9C] != 0;
			Console.WriteLine("PAL60 enabled: " + pal60);

			// Language
			byte lang = bytes[0x9D];
			Console.WriteLine("Language: " + lang);

			// JIT branch following
			bool jitBranchFollow = bytes[0x9F] != 0;
			Console.WriteLine("JIT branch following: " + jitBranchFollow);

			// Name of second disc iso
			string discIso = Encoding.UTF8.GetString(bytes, 0xA9, 40);
			Console.WriteLine("Name of second disc ISO: " + discIso);

			// SHA-1 hash of Dolphin Git revision
			byte[] gitSha1 = new byte[20];
			Array.Copy(bytes, 0xD1, gitSha1, 0, 20);
			string gitSha1Str = "";
			foreach (byte b in gitSha1) gitSha1Str += b.ToString("X2");
			Console.WriteLine("Dolphin SHA-1 Git revision: " + gitSha1Str);

			// DSP IROM hash
			int dspIrom = BitConverter.ToInt32(bytes, 0xE5);
			Console.WriteLine("DSP IROM hash: " + dspIrom.ToString("X32"));

			// DSP COEF hash
			int dspCoef = BitConverter.ToInt32(bytes, 0xE9);
			Console.WriteLine("DSP COEF hash: " + dspCoef.ToString("X32"));

			// Tick count
			long ticks = BitConverter.ToInt64(bytes, 0xED);
			Console.WriteLine("Tick count: " + ticks);
		}
		public static string FileError(string file, string problem) { return "File '" + file + "' " + problem; }
	}
}