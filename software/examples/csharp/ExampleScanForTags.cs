using Tinkerforge;

class Example
{
	private static string HOST = "localhost";
	private static int PORT = 4223;
	private static string UID = "XYZ"; // Change to your UID

	private static byte currentTagType = BrickletNFCRFID.TAG_TYPE_MIFARE_CLASSIC;

	// Callback function for state changed callback
	static void StateChangedCB(BrickletNFCRFID sender, byte state, bool idle)
	{
		if(idle)
		{
			currentTagType = (byte)((currentTagType + 1) % 3);
			sender.RequestTagID(currentTagType);
		}

		if(state == BrickletNFCRFID.STATE_REQUEST_TAG_ID_READY)
		{
			byte tagType;
			byte tidLength;
			byte[] tid;

			sender.GetTagID(out tagType, out tidLength, out tid);

			if(tidLength == 7)
			{
				System.Console.WriteLine("Found tag of type {0} with ID " +
				                         "[{1:X} {2:X} {3:X} {4:X} {5:X} {6:X} {7:X}]",
				                         tagType, tid[0], tid[1], tid[2], tid[3], tid[4], tid[5], tid[6]);
			}
			else
			{
				System.Console.WriteLine("Found tag of type {0} with ID [{1:X} {2:X} {3:X} {4:X}]",
				                         tagType, tid[0], tid[1], tid[2], tid[3]);
			}
		}
	}

	static void Main()
	{
		IPConnection ipcon = new IPConnection(); // Create IP connection
		BrickletNFCRFID nfcrfid = new BrickletNFCRFID(UID, ipcon); // Create device object

		ipcon.Connect(HOST, PORT); // Connect to brickd
		// Don't use device before ipcon is connected

		// Register state changed callback to function StateChangedCB
		nfcrfid.StateChanged += StateChangedCB;

		nfcrfid.RequestTagID(BrickletNFCRFID.TAG_TYPE_MIFARE_CLASSIC);

		System.Console.WriteLine("Press enter to exit");
		System.Console.ReadLine();
		ipcon.Disconnect();
	}
}
