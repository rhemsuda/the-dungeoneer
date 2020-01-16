using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomConnector 
{
	public GameObject Connector;
	public GameObject Opening;
    public GameObject Wall;
	public int ConnectorNumber;

	public RoomConnector(GameObject connector, GameObject opening, GameObject wall, int connectorNumber)
	{
		this.Connector = connector;
		this.Opening = opening;
        this.Wall = wall;
		this.ConnectorNumber = connectorNumber;
	}
}
