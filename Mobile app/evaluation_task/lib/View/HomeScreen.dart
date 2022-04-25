import 'package:evaluation_task/View/CameraScreen.dart';
import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:camera/camera.dart';
import 'package:flutter/rendering.dart';
class HomeScreen extends StatefulWidget {
 var cameras;
 HomeScreen(this.cameras);



  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  late CameraController controller;
  @override


  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title:Center(child: Text("AR Appication")),
      ),
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.spaceEvenly,
          crossAxisAlignment: CrossAxisAlignment.center,
          children: [
            Text(
              "Evaluation Task",
              style: TextStyle(fontSize: 30, fontWeight: FontWeight.bold),
            ),
            Padding(
              padding: const EdgeInsets.all(8.0),
              child: Center(child: Text("This is A Test APP for AR That you can check if the two picture are related to each other or not",style: TextStyle(fontSize: 15),textAlign: TextAlign.center,)),
            )
            ,Container(
              color: Colors.white,
              child: ElevatedButton(onPressed: () {
               Navigator.push(context, MaterialPageRoute(builder: (BuildContext context) {
                 return CameraScreen(widget.cameras);
               }));
              }, child: Text("Open Camera",style: TextStyle(color: Colors.blue,fontSize: 12),),style: ButtonStyle(
                backgroundColor: MaterialStateProperty.all(Colors.yellow),
              )),
            )
          ],
        ),
      ),
    );
  }
}
