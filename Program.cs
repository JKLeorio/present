using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Geolocation {

public class Program {
	public static void Main(string[] args){
		if (ReadData.GetJson().Count==0){
			Console.WriteLine("no data, write data for program with command (Add)");
		}
		if ( args.Length > 0 & args.Length < 5){
			switch(args[0].ToLower()) {
				case "get" :{
					Access_interface.Get(args[1]);
					break;
				}
				case "add" :{
					Access_interface.Add(args);
					break;
				}
				case "move" :{
					Access_interface.Move(args);
					break;
				}
			}
		}
	}
}



public class Access_interface{
	public static void Get(string Val){
		int Wvar=PathState.DistanceTo(Val);
		if (Wvar==-1){
			Console.WriteLine("Data have not this reference point");
		}
		else {
			Console.WriteLine(Wvar);
		}
	}
	public static void Add(string[] Arr){
		int dse= int.Parse(Arr[2]);
		WriteData.WriteJson(new PathState(Arr[1],dse));
	}
	public static void Move(string[] Arr){
		int speed = int.Parse(Arr[1]);
		int time = int.Parse(Arr[2]);
		Console.WriteLine(PathState.Move(speed, time, Arr[3]));
	}
}


class PathState {
	public string place{
		get;
		set;
	}
	public int distance_to_start_place {
		get;
		set;
	}
	public int distance_to_place{
		get;
		set;
	}
	public string start_place{
		get;
		set;
	}

	public PathState(string place, int distance_to_start_place, int distance_to_place=0){
		this.place = place;
		this.distance_to_start_place = distance_to_start_place;
		this.distance_to_place = distance_to_place;
		this.start_place = "Osh";
	}

	public static string Move(int speed, int time, string ReferencePoint){
		int output;
		foreach(PathState ob in ReadData.GetJson()){
			if (ob.place.ToLower() == ReferencePoint.ToLower()){
				output=ob.distance_to_start_place-(speed * time);
				if (output<0) {
					return "Distance to destination = 0 ";
				}
				return $"Distance to destination = {output}";
			}
		}
		return "Data have not this reference point";
	}

	public static int DistanceTo(string place){
		foreach(PathState ob in ReadData.GetJson()){
			if (ob.place.ToLower()==place.ToLower()) {
				return ob.distance_to_start_place;
			}
		}
		return -1;
	}

}

class PathToFile {
	public static string path_to_file = @"data.json";
}


class WriteData : PathToFile {
	public static void WriteJson(PathState Pob){
		string serilize= JsonSerializer.Serialize(Pob)+"|";
		List<PathState> obArr = ReadData.GetJson();
		for(int i=0; i<obArr.Count ; i++){
			if (obArr[i].place.ToLower() == Pob.place.ToLower()){
				serilize="";
				break;
			}
		}
		using(FileStream fs = new FileStream(path_to_file, FileMode.Create)){
			foreach(PathState ob in obArr){
				serilize+=JsonSerializer.Serialize(ob)+"|";
			}
			byte[] buffer = Encoding.Default.GetBytes(serilize);
			fs.Write(buffer, 0 , buffer.Length);
		}
	}
}

class ReadData : PathToFile {
	public static List<PathState> GetJson(){
		List<PathState> pCollection = new List<PathState>();
		using(FileStream fs = new FileStream(path_to_file, FileMode.OpenOrCreate)){
			byte[] buffer = new byte[fs.Length];
			fs.Read(buffer,0, buffer.Length);
			string[] arr= Encoding.Default.GetString(buffer).Split("|");
				foreach(string str in arr) {
					if(str!=""){
						PathState? ps = JsonSerializer.Deserialize<PathState>(str);
						if(ps!=null){
							pCollection.Add(ps);
						}
					}
				}
			}
		return pCollection;
		}
	}
}