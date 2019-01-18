// Teleportal SDK
// Code by Thomas Suarez
// Copyright 2018 WiTag Inc

/// <summary>
/// Teleportal Network Commands
/// </summary>
public class TeleportalCmd {

	public static string API_KEY = "a0";
	public static string API_KEY_OK = "a1";
	public static string API_KEY_BAD = "a2";
	public static string BETA_AUTHORIZED = "a3";
	public static string BETA_UNAUTHORIZED = "a4";
	public static string APP_IN_REVIEW = "a5";
	public static string HOOK_ADD = "a6";
	public static string HOOK_REMOVE = "a7";
	public static string DIRECT_IP = "a8";
	public static string HEARTBEAT = "a9";
	public static string SOCKET_UUID = "a11";

	public static string AUTH_REGISTER = "b0";
	public static string AUTH_REGISTER_OK = "b1";
	public static string AUTH_REGISTER_BAD = "b2";
	public static string AUTH_REGISTER_UPDATE_OK = "b3";

	public static string AUTH_LOGIN = "b4";
	public static string AUTH_LOGIN_OK = "b5";
	public static string AUTH_LOGIN_BAD = "b6";
	public static string AUTH_LOGIN_GET = "b7";
	public static string AUTH_LOGIN_GET_OK = "b8";
	public static string AUTH_LOGIN_GET_BAD = "b9";

	public static string AUTH_DISCONNECT = "b10";
	public static string AUTH_VIA_WEB = "b11";

	public static string MODULE_DOWNLOAD = "c0";
	public static string MODULE_DOWNLOAD_OK = "c1";
	public static string MODULE_DOWNLOAD_BAD = "c2";
	public static string MODULE_PURCHASE = "c3";
	public static string MODULE_PURCHASE_OK = "c4";
	public static string MODULE_PURCHASE_BAD = "c5";
	public static string MODULE_CONNECT = "c6";
	public static string MODULE_DISCONNECT = "c7";

	public static string FRIEND_ADD = "d0";
	public static string FRIEND_ADD_OK = "d1";
	public static string FRIEND_ADD_BAD = "d2";
	public static string FRIEND_SEARCH = "d3";
	public static string FRIEND_LIST = "d4";
	public static string FRIEND_LIST_OK = "d5";
	public static string FRIEND_NOTIFY = "d6";

	public static string LOCATION_UPDATE_TOKEN = "e0";
	public static string LOCATION_ORIGIN_USER = "e1";
	public static string LOCATION_ORIGIN_AREA = "e2";
	public static string LOCATION_SYNC = "e3";
	public static string LOCATION_UPDATE = "e4";
	public static string LOCATION_TELEPORT = "e5";
	public static string LOCATION_POINT_HEADINGS = "e7";
	public static string LOCATION_TRACKING_RESET = "e8";

	public static string META_AVATAR = "f1";
	public static string META_HIGHFIVE = "f2";
	public static string META_HIGHFIVE_SENT = "f3";
	public static string META_INVENTORY = "f4";
	public static string META_INVENTORY_ADD = "f5";

	public static string ITEM_USE = "g0";
    public static string ITEM_ADD = "g1";
	public static string ITEM_MOVE = "g2";
	public static string ITEM_DELETE = "g3";
	public static string ITEM_PHYSICS_UPDATE = "g4";
	public static string ITEM_REPOSITION = "g5";
	public static string ITEM_STATE = "g6";

    public static string REALM_CREATE = "h0";
    public static string REALM_JOIN = "h1";
    public static string REALM_MOVE = "h2";
    public static string REALM_DELETE = "h3";
    public static string REALM_ITEM_ADD = "h4";

	public static string POINTCLOUD_ADD = "i0";
	public static string POINTCLOUD_RESET = "i1";

	public static string DEBUG_MODE = "z0";
	public static string DEBUG_TEXT_SEND = "z1";
	public static string DEBUG_TEXT_ACK = "z2";

	public static string UDP_LOCATION = "a";
	public static string UDP_ACCELERATION = "b";

}

public enum AuthProvider {
	Simple, Google, Digits
}

public enum AvatarType {
	None, AJ, Cat, Tiger
}