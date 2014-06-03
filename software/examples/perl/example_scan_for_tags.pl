#!/usr/bin/perl  

use Tinkerforge::IPConnection;
use Tinkerforge::BrickletNFCRFID;

use constant HOST => 'localhost';
use constant PORT => 4223;
use constant UID => 'hjw'; # Change to your UID

my $ipcon = Tinkerforge::IPConnection->new(); # Create IP connection
my $nfc = Tinkerforge::BrickletNFCRFID->new(&UID, $ipcon); # Create device object
my $tag_type = 0;

# Callback function for state changed callback
sub cb_state_changed
{
	my ($state, $idle) = @_;

	if($idle) {
		$tag_type = ($tag_type + 1) % 3;
		$nfc->request_tag_id($tag_type);
	}

	if($state == $nfc->STATE_REQUEST_TAG_ID_READY) {
		my ($tag_type_new, $tid_length, $tid) = $nfc->get_tag_id();
		my $s = "Found tag of type " . $tag_type . " with ID [" . sprintf("%x", @{$tid}[0]);
		for($i = 1; $i < $tid_length; $i++) {
			$s = $s . ", " . sprintf("%x", @{$tid}[$i]);
		}
		$s = $s . "]\n";

		print $s;
	}
}

$ipcon->connect(&HOST, &PORT); # Connect to brickd
# Don't use device before ipcon is connected

# Register state changed callback to function cb_state_changed
$nfc->register_callback($nfc->CALLBACK_STATE_CHANGED, 'cb_state_changed');

$nfc->request_tag_id($nfc->TAG_TYPE_MIFARE_CLASSIC);

print "Press any key to exit...\n";
<STDIN>;
$ipcon->disconnect();
