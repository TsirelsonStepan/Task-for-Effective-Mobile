static class Configuration
{
    static string _configFilePath = "config";
    public static void LoadConfigData()
    {
        using StreamReader reader = new StreamReader(_configFilePath);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (line == null) continue;
            string variable = line.Split('=')[0];
            string value = line.Split('=')[1];
            if (variable == "_deliveryLog") Program._deliveryLog = value;
            else if (variable == "_deliveryOrder") Program._deliveryOrder = value;
            else if (variable == "_cityDistrict") Program._cityDistrict = value;
            else if (variable == "_firstDeliveryDateTime") Program._firstDeliveryDateTime = value;
        }
    }

    static void SaveConfigData()
    {
        using StreamWriter writer = new StreamWriter(_configFilePath);
        writer.WriteLine($"_deliveryLog={Program._deliveryLog}");
        writer.WriteLine($"_deliveryOrder={Program._deliveryOrder}");
        writer.WriteLine($"_cityDistrict={Program._cityDistrict}");
        writer.WriteLine($"_firstDeliveryDateTime={Program._firstDeliveryDateTime}");
    }        
}