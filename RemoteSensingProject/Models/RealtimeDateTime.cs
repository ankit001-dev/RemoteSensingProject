// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// RemoteSensingProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// RemoteSensingProject.Models.RealtimeDateTime
using System;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

public class RealtimeDateTime : Hub
{
	public void SendRealTimeDateTime()
	{
		string currentDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
		((dynamic)((IHubConnectionContext<object>)(object)((Hub)this).Clients).All).receiveTime(currentDateTime);
	}
}
