//using System.Collections.Generic;
//using System;

//using UnityEngine;
//using UnityEngine.UI;

//using Melanchall.DryWetMidi.Interaction;

//namespace NagaisoraFramework.Media
//{
//	public enum DisplayMode
//	{
//		Color,
//		Alpha,
//	}


//	public class MidiPianoControler : MonoBehaviour
//	{
//		public Note[] notes;

//		public GameObject Piano;
//		public GameObject NoteObject;

//		public DisplayMode DisplayMode;

//		GameObject[] objects;

//		Image[] objectsImage;

//		public List<GameObject> NoteObjects = new List<GameObject>();
//		public List<Image> NoteImage = new List<Image>();

//		public void Start()
//		{
//			for (int i = 0; i < 127; i++)
//			{
//				GameObject NotesObject = Instantiate(NoteObject);
//				NotesObject.transform.SetParent(Piano.transform);
//				RectTransform rectTransform = NotesObject.GetComponent<RectTransform>();
//				rectTransform.sizeDelta = new Vector2((800 / 127), 0);
//				NotesObject.transform.localScale = new Vector3(1, 1, 1);
//				NotesObject.transform.localPosition = new Vector3((-(800 / 2)) + ((800 / 127) * i), 0, 0);
//				NotesObject.SetActive(false);
//				NoteObjects.Add(NotesObject);
//				NoteImage.Add(NoteObject.GetComponent<Image>());
//			}

//			objects = NoteObjects.ToArray();
//			objectsImage = NoteImage.ToArray();
//		}

//		public void FixedUpdate()
//		{
//			foreach (GameObject gameObject in objects)
//			{
//				gameObject.SetActive(false);
//			}

//			if (notes != null)
//			{
//				foreach (Note note in notes)
//				{
//					GameObject NObject = objects[note.NoteNumber];

//					if (DisplayMode == DisplayMode.Color)
//					{
//						NObject.GetComponent<Image>().color = new Color(Convert.ToSingle(note.Velocity) / 127f, 0f, 127f - Convert.ToSingle(note.Velocity) / 127f, 100f / 255f);
//					}
//					else
//					{
//						NObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, Convert.ToSingle(note.Velocity) / 127f);
//					}

//					NObject.SetActive(true);
//				}
//			}
//		}
//	}
//}