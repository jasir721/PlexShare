/***************************
 * Filename    = WhiteBoardViewModel.cs
 *
 * Author      = Aiswarya H
 *
 * Product     = Plex Share
 * 
 * Project     = White Board
 *
 * Description = This is the Undo Redo Implementation.
 *               This contains the undo and redo functions which are
 *               a part of the ViewModel. 
 ***************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using PlexShareWhiteboard.BoardComponents;
using PlexShareWhiteboard.Client;

namespace PlexShareWhiteboard
{
    public partial class WhiteBoardViewModel
    {

        /// <summary>
        /// This function is called when a user clicks on Undo.
        /// 1. A helper function to call the Undo function 
        /// 2. Takes the object returned by Undo to get the object and operation
        /// 3. Sends to the Server the operation that needs to be performed and on which object the operation has to be performed
        /// </summary>
        public void CallUndo()
        {
            if (modeForUndo == "create_textbox")
            {
                if (textBoxLastShape != null && textBoxLastShape.TextString != null &&
                         textBoxLastShape.TextString.Length != 0)
                {

                    TextFinishPush();
                    Debug.WriteLine("entering undo modeeeee");

                }
                else if (textBoxLastShape != null)
                {
                    for (int i = 0; i < ShapeItems.Count; ++i)
                    {
                        if (textBoxLastShape.Id == ShapeItems[i].Id)
                        {
                            ShapeItems.RemoveAt(i);
                            break;
                        }
                    }
                }
            }

            UndoStackElement shapeToSend = Undo();
            if (shapeToSend != null)
                machine.OnShapeReceived(shapeToSend.PrvShape, shapeToSend.Op);
        }

        /// <summary>
        /// This function is called when a user clicks on Redo.
        /// 1. A helper function to call the Redo function 
        /// 2. Takes the object returned by Redo to get the object and operation
        /// 3. Sends to the Server the operation that needs to be performed and on which object the operation has to be performed
        /// </summary>
        public void CallRedo()
        {
            /*if (mode == "create_textbox")
            {
                if (lastShape.TextString.Length != 0)
                {
                    TextFinishPush();
                }
            }*/
            UndoStackElement shapeToSend = Redo();
            if (shapeToSend != null)
                machine.OnShapeReceived(shapeToSend.NewShape, shapeToSend.Op);
        }


        // Initialising Stacks for Undo and Redo
        public Stack<UndoStackElement> undoStack = new Stack<UndoStackElement>();
        public Stack<UndoStackElement> redoStack = new Stack<UndoStackElement>();


        /// <summary>
        /// To perform Undo operation
        /// 1. Pop the UndoStack to get the top of the Stack
        /// 2. Save a deep copy of the object as modifiedObject. 
        /// Basically,
        /// topOfStack -> pushed to Redo Stack
        /// modifiedObject -> to be sent to Server
        /// 3. Perform the inverse of the operation associated with the object. The modifiedObject's operation is set as the inverse operation
        /// (as this has to be communicated to the other clients through the server)
        /// 4. The topOfStack is pushed to the RedoStack.
        /// 5. The modifiedObject is then returned to the helper function 
        /// </summary>
        /// <returns>UndoStackElement containing the shape and the operation to be broadcasted</returns>
        public UndoStackElement Undo()
        {
            if (undoStack.Count == 0)   // Undo Stack Empty Case
                return null;

            UndoStackElement topOfStack = undoStack.Pop();
            if (topOfStack.PrvShape == null)
                return null;
            UndoStackElement modifiedObject = new UndoStackElement(topOfStack.PrvShape, topOfStack.NewShape, topOfStack.Op);


            Trace.WriteLine("[Whiteboard]  " + "\n" + topOfStack.Op + "\n");

            /* Depending on the operation, perform the inverse opreation 
            by calling appropriate functions to modify ShapeList */

            switch (topOfStack.Op)
            {
                case Operation.Creation:
                    DeleteIncomingShape(topOfStack.PrvShape);
                    modifiedObject.Op = Operation.Deletion;
                    break;
                case Operation.Deletion:
                    CreateIncomingShape(topOfStack.PrvShape);
                    modifiedObject.Op = Operation.Creation;
                    break;
                case Operation.ModifyShape:
                    ModifyIncomingShape(topOfStack.PrvShape);
                    modifiedObject.Op = Operation.ModifyShape;
                    break;
            }
            redoStack.Push(topOfStack);
            Trace.WriteLine("[Whiteboard]  " + "\n" + redoStack.Peek().Op + " is pushed to Redo Stack \n");
            return modifiedObject;
        }

        /// <summary>
        /// To perform Redo operation
        /// 1. Pop the RedoStack to get the top of the Stack
        /// 2. Perform the operation associated with the object.
        /// 3. The topOfStack is pushed to the UndoStack.
        /// 5. topOfStack is then returned to the helper function 
        /// </summary>
        /// <returns>UndoStackElement containing the shape and the operation to be broadcasted</returns>
        public UndoStackElement Redo()
        {
            if (redoStack.Count == 0)
                return null;
            
            UndoStackElement topOfStack = redoStack.Pop();
            if (topOfStack.NewShape == null)
                return null;

            switch (topOfStack.Op)
            {
                case Operation.Creation:
                    Trace.WriteLine("[Whiteboard]  " + "\n Redo Creation " + topOfStack.NewShape.Id + "\n");
                    CreateIncomingShape(topOfStack.NewShape);
                    break;
                case Operation.Deletion:
                    Trace.WriteLine("[Whiteboard]  " + "\n Redo Deletion " + topOfStack.NewShape.Id + "\n");
                    DeleteIncomingShape(topOfStack.NewShape);
                    break;
                case Operation.ModifyShape:
                    Trace.WriteLine("[Whiteboard]  " + "\n Redo ModifyShape " + topOfStack.NewShape.Id + "\n");
                    ModifyIncomingShape(topOfStack.NewShape);
                    break;
            }
            undoStack.Push(topOfStack);
            Trace.WriteLine("[Whiteboard]  " + "\n " + undoStack.Peek().NewShape.Id + " is pushed to UndoStack \n");
            return topOfStack;
        }

        /// <summary>
        /// To insert a ShapeItem along with operation perfomed whenever
        /// a client performs an action on WhiteBoard
        /// </summary>
        /// <param name="obj">UndoStackElement to be pushed on UndoStack</param>
        public void InsertIntoStack(UndoStackElement obj)
        {
            undoStack.Push(obj);
            Debug.WriteLine(obj.Op + " Operation" + obj.NewShape.Id + "id inserted\n");
        }

    }
}
