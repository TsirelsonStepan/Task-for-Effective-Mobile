struct Date
{
	public int year = -1;
	public int month = -1;
	public int day = -1;
	public Date(string[] date_str)
	{
		int.TryParse(date_str[0], out year);
		int.TryParse(date_str[1], out month);
		int.TryParse(date_str[2], out day);
	}
}

public static class Program
{
	public static string _inputFilePath = "data.csv";
	public static string _deliveryLog = ".";
	public static string _deliveryOrder = ".";
	public static string _cityDistrict = "";
	public static string _firstDeliveryDateTime = "";
	static Date firstDeliveryDate;
	static int firstDeliveryTime;

	public static void Main()
	{
		Configuration.LoadConfigData();

		Console.WriteLine("(0) Current district to filter by: " + _cityDistrict);
		Console.WriteLine("(1) Date and time of first delivery: " + _firstDeliveryDateTime);
		Console.WriteLine("(2) You can find operation log here: " + _deliveryLog);
		Console.WriteLine("(3) The result of filtering is located here " + _deliveryOrder);

		while (true)
		{
			if (!ProcessInput()) return;
			if (ValidateInput()) break;
		}

		List<string> filteredList = FilterData();
		foreach (string x in filteredList)
		{	
			Console.WriteLine(x);
		}
		if (filteredList.Count == 0) Console.WriteLine("None were found");
		Console.WriteLine("To finish execution press any button...");
		Console.Read();
	}

	static List<string> FilterData()
	{
		List<string> filteredList = new List<string>();
		using StreamReader reader = new StreamReader(_inputFilePath);
		var header = reader.ReadLine();
		string[] colNames;
		if (header == null)
		{
			Console.WriteLine("Header not of type \"OrderID,Mass(kg),DistrictID,DateTime\". Unable to proceed");
			return filteredList;
		}
		else colNames = header.Split(',');
		
		int rowsCounter = 0;
		while (!reader.EndOfStream)
		{
			var line = reader.ReadLine();
			rowsCounter++;
			string[] values = new string[4];
			if (line == null) continue;
			values = line.Split(',');
			bool correctDistrict = values[2] == _cityDistrict;

			string[] dateTimeArray = values[3].Split(' ');
			int thisTime = GetUniversalTime(dateTimeArray[1]);
			if (thisTime == -1)
			{
				Console.WriteLine("Wrong time format. Skipping line");
				continue;
			}
			string strDate = dateTimeArray[0];
			if (strDate.Split('-').Length != 3)
			{
				Console.WriteLine($"Wrong date format in line {rowsCounter}. Skipping line.");
				continue;
			}
			Date thisDate = new Date(strDate.Split('-'));

			if (correctDistrict)
			{
				if (thisDate.year == firstDeliveryDate.year && thisDate.month == firstDeliveryDate.month && thisDate.day == firstDeliveryDate.day && Math.Abs(thisTime - firstDeliveryTime) <= 30 * 60)
				{
					filteredList.Add(line);
				}
			}
			
		}
		return filteredList;
	}

	static bool ValidateInput()
	{
		bool error = false;
		if (_deliveryLog == ".") Console.WriteLine("Warning: Path for log is not specified. Log will be placed in the app directory");
		else if (!Directory.Exists(_deliveryLog)) { Console.WriteLine("Input error: Path to log does not exist"); error = true; }
		if (_deliveryOrder == ".") Console.WriteLine("Warning: Path for filtered orders is not specified. Filtered list will be placed in the app directory");
		else if (!Directory.Exists(_deliveryOrder)) { Console.WriteLine("Input error: Output path does not exist"); error = true; }
		if (_cityDistrict == "") { Console.WriteLine("Input error: District not specified"); error = true; }
		if (_firstDeliveryDateTime == "") { Console.WriteLine("Input error: First order date and time is not specified"); error = true; }
		else
		{
			string[] date = _firstDeliveryDateTime.Split(' ')[0].Split('-');
			string[] time = _firstDeliveryDateTime.Split(' ')[1].Split(':');
			if (date.Length != 3 || time.Length != 3) { Console.WriteLine("Input error: Wrong date and time format"); error = true; }
			else
			{
				firstDeliveryDate = new Date(date);
				if (firstDeliveryDate.day == -1 || firstDeliveryDate.month == -1 || firstDeliveryDate.year == -1) Console.WriteLine("Input error: Wrong date and time format"); error = true;
				int[] timeInts = new int[3];
				bool breaker = true;
				for (int i = 0; i < 3; i++)
					if (!int.TryParse(time[i], out timeInts[i])) breaker = false;
				
				if (breaker) firstDeliveryTime = timeInts[0] * 3600 + timeInts[1] * 60 + timeInts[2];
				else Console.WriteLine("Input error: Wrong date and time format"); error = true;
			}
		}
		return error;
	}

	static bool ProcessInput()
	{
		while (true)
		{
			Console.WriteLine("If you want to change settings, provide number of variable and new value separated by \'=\' sign.\nExample: \"1=2024-10-28 00:20:12\"\nTo exit type \"exit\". To start filtering type: \"filter\"");
			var input = Console.ReadLine();
			if (input == "exit") return false;
			else if (input == "filter") return true;
			else if (input == null || input.Split('=').Length != 2)
			{
				Console.WriteLine("Incorrect input");
				continue;
			}
			else
			{
				string index = input.Split('=')[0];
				string value = input.Split('=')[1];
				if (index == "0") _cityDistrict = value;
				else if (index == "1") _firstDeliveryDateTime = value;
				else if (index == "2") _deliveryLog = value;
				else if (index == "3") _deliveryOrder = value;
			}
		}
	}

	public static int GetUniversalTime(string input)
	{
		string[] timeParts = input.Split(':');
		if (timeParts.Length != 3) return -1;
		int[] timeIntParts = new int[3];
		bool breaker = true;
		for (int i = 0; i < 3; i++) if (!int.TryParse(timeParts[i], out timeIntParts[i])) breaker = false;
		if (breaker) return timeIntParts[0] * 3600 + timeIntParts[1] * 60 + timeIntParts[2];
		return -1;
	}
}