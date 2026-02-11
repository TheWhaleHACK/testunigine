using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "5f66957eaa90f21da5f3e64f7b13304582cddfd5")]
public class TestTrinner : Component
{
	private WorldTrigger trigger;
	
	public bool isConnected = false;

	void Init()
	{
		// write here code to be called on component initialization
		trigger = node as WorldTrigger;
        if (trigger != null)
        {
            trigger.EventEnter.Connect( OnTriggerEnter); // Используем += для подключения делегата
            trigger.EventLeave.Connect( OnTriggerLeave); // Используем += для подключения делегата
        }
        else
        {
            Log.Error("OpenNextWindow: Node is not a WorldTrigger! Node: {0}\n", node.Name);
        }
	}
	

	private void OnTriggerEnter(Node entered_node)
	{
		if (!isConnected)
			isConnected = true;
	}

	private void OnTriggerLeave(Node entered_node)
	{
		if (isConnected)
			isConnected = false;
			
	}
	void ConsoleTest()
	{
		Log.MessageLine("dsafewefwfwfwef");
	}
}