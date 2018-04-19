using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// author: mvaganov@hotmail.com
// license: Copyfree, public domain. This is free code! Great artists, steal this code!
// latest version at: https://pastebin.com/raw/DCYtMFrp
public class GenerateMap : MonoBehaviour {
	[System.Serializable]
	public class BuildingBlock
	{
		public string character;
		public GameObject blockObject;
	}
	public BuildingBlock[] buildingBlocks;
	private Vector3Int indexOffset = Vector3Int.zero;
	private Vector3Int calculatedSize = Vector3Int.zero;
	[TextArea(10,10)]
	public string map =
		"#######\n" +
		".....##\n" +
		".....#\n" +
		"#\n" +
		"#....#..........#\n" +
		"###.###\n" +
		"..#.###\n" +
		".\n" +
		"####............#\n" +
		"\n" +
		"#.....###\n" +
		"#.......#..###\n" +
		"#.......#..#.#\n" +
		"#..........###\n" +
		"............#...#\n" +
		"............#...#\n" +
		"#..##############\n" +
		"#..#............#\n" +
		"#..#............#";
	private char[,,] mutableData;
	private GameObject[,,] allBlockObjects;
	/// <summary>which index is dirty</summary>
	private List<Vector3Int> dirtyList = new List<Vector3Int>();
	public Vector3 cubeSize = new Vector3(3,3,3);


	/// <summary>which index the user mouse is pointing at</summary>
	public Vector3Int selectIndex;
	/// <summary>which index is the creation point</summary>
	public Vector3Int createIndex;
	public string leftClickAdds = "#";
	public bool enableFreeEdit = false;
	public bool rightClickRemoves = true;
	public bool autoExpand = true;
	public int expansionBorderSize = 1;

	public char GetDataAt(int level, int row, int col) { return GetDataAt(new Vector3Int(col, row, level)); }
	public char GetDataAt(Vector3Int index) {
		index -= indexOffset;
		return mutableData [index.z, index.y, index.x];
	}
	public void SetDataAt(int level, int row, int col, char c) { SetDataAt(new Vector3Int(col, row, level), c); }
	public void SetDataAt(Vector3Int index, char c) {
		index -= indexOffset;
//		Debug.Log (index);
		mutableData [index.z, index.y, index.x] = c;
	}

	public GameObject GetObjectAt(int level, int row, int col) { return GetObjectAt(new Vector3Int(level, row, col)); }
	public GameObject GetObjectAt(Vector3Int index) {
		index -= indexOffset;
		return allBlockObjects [index.z, index.y, index.x];
	}
	public void SetObjectAt(int level, int row, int col, GameObject go) { SetObjectAt(new Vector3Int(level, row, col), go); }
	public void SetObjectAt(Vector3Int index, GameObject go) {
		index -= indexOffset;
		allBlockObjects [index.z, index.y, index.x] = go;
	}

	[ContextMenu("Recalculate Bounds")]
	void RecalculateBounds() {
		// create the box collider, which will help position the generator in the game world
		BoxCollider b = GetComponent<BoxCollider> ();
		if (b == null) {
			b = gameObject.AddComponent<BoxCollider> ();
			b.isTrigger = true;
			gameObject.layer = LayerMask.NameToLayer ("Ignore Raycast");;
		}
		calculatedSize = CalculateMapSize ();
		Vector3 s = (Vector3)calculatedSize;
		s.x = s.x * cubeSize.x;
		s.y = s.z * cubeSize.y;
		s.z = -calculatedSize.y * cubeSize.z;
		b.size = s;
		Vector3 offset = cubeSize / -2;
		offset.z *= -1;
		b.center = b.size / 2 + offset;
		// ensure there is an entry for each character in the map
		string letters = LettersUsedInMap ();
		if (buildingBlocks == null) {
			buildingBlocks = new BuildingBlock[0];
		}
		for (int i = 0; i < letters.Length; ++i) {
			if (GetBlock (letters [i]) == null) {
				int newSize = buildingBlocks.Length + 1;
				System.Array.Resize (ref buildingBlocks, newSize);
				BuildingBlock block = new BuildingBlock ();
				block.character = letters [i].ToString();
				buildingBlocks [buildingBlocks.Length - 1] = block;
			}
		}
	}

	public char FindCharAtFromOriginalMap(Vector3Int index) {
		if (calculatedSize == Vector3Int.zero) {
			calculatedSize = CalculateMapSize ();
		}
		if (index.x >= 0 && index.y < calculatedSize.x &&
			index.y >= 0 && index.y < calculatedSize.y &&
			index.z >= 0 && index.y < calculatedSize.z) {
			int row = 0, col = 0, dep = 0;
			for (int i = 0; i < map.Length; i++) {
				char c = map [i];
				if (c == '\r') { continue; }
				else if (c == '\n') { if (col == 0) { dep++; row = 0; if (dep > index.z) { break; } } else { row++; } col = 0; }
				else {
					if (col == index.x && row == index.y && dep == index.z) { return c; }
				}
			}
		}
		return (char)0;
	}

	public string LettersUsedInMap() {
		string output = "";
		for (int i = 0; i < map.Length; ++i) {
			if (map [i] != '\n' && output.IndexOf (map [i]) < 0) {
				output += map [i];
			}
		}
		return output;
	}

	public string ToText(char emptyArea) {
		System.Text.StringBuilder sb = new System.Text.StringBuilder ();
		for (int dep = 0; dep < calculatedSize.z; dep++) {
			for (int row = 0; row < calculatedSize.y; ++row) {
				int lastUsedIndex = -1;
				char c;
				for (int col = 0; col < calculatedSize.x; ++col) {
					c = GetDataAt(dep, row, col);
					if (c != (char)0 && c != emptyArea) {
						lastUsedIndex = col;
					}
				}
				if (lastUsedIndex < 0) {
					sb.Append (emptyArea);
				} else {
					for (int col = 0; col <= lastUsedIndex; ++col) {
						c = GetDataAt(dep, row, col);
						if (c == 0) {
							c = emptyArea;
						}
						sb.Append (c);
					}
				}
				sb.Append ("\n");
			}
			sb.Append ("\n");
		}
		return sb.ToString ();
	}

	public GenerateMap ContainsRecursiveGenerator(List<GenerateMap> currentGenerators = null){
		if (currentGenerators == null) {
			currentGenerators = new List<GenerateMap> ();
		}
		if (currentGenerators.Contains (this)) {
			return this;
		}
		for (int i = 0; i < buildingBlocks.Length; ++i) {
			if (buildingBlocks [i].blockObject != null) {
				GenerateMap[] generators = buildingBlocks [i].blockObject.GetComponents<GenerateMap> ();
				if (generators != null) {
					for (int g = 0; g < generators.Length; ++g) {
						if (generators [g].ContainsRecursiveGenerator (currentGenerators)) {
							return generators [g];
						}
					}
				}
			}
		}
		return null;
	}

	// Use this for initialization
	void Start () {
		GenerateMap recursiveMap = ContainsRecursiveGenerator ();
		if (recursiveMap != null) {
			throw new System.Exception ("ERROR: about to infinite-recursively create objects with " + recursiveMap);
		}
		if (calculatedSize == Vector3Int.zero) {
			calculatedSize = CalculateMapSize ();
		}
		SetMutableDataFrom (map);
		RefreshMap (true);
		Debug.Log(ToText ('.'));
	}
	void SetMutableDataFrom(string map) {
		mutableData = new char[calculatedSize.z, calculatedSize.y, calculatedSize.x];
		int row = 0, col = 0, dep = 0;
		for (int index = 0; index < map.Length; index++) {
			char c = map [index];
			if (c == '\r') { continue; }
			if (c == '\n') {
				if (col == 0) {
					dep++; row = 0;
				} else {
					row++;
				}
				col = 0;
			} else {
				SetDataAt(dep, row, col, c);
				col++;
			}
		}
	}
	public void DeleteAll() {
		Vector3Int i = Vector3Int.zero;
		for (i.z = 0; i.z < allBlockObjects.GetLength (0); i.z++) {
			for (i.y = 0; i.y < allBlockObjects.GetLength (1); i.y++) {
				for (i.x = 0; i.x < allBlockObjects.GetLength (2); i.x++) {
					Destroy (GetObjectAt(i));
					SetObjectAt (i, null);
				}
			}
		}
	}
	public BuildingBlock GetBlock(char c){
		if (buildingBlocks != null) {
			for (int b = 0; b < buildingBlocks.Length; ++b) {
				BuildingBlock block = buildingBlocks [b];
				if (block.character != null && block.character.Length > 0 && block.character [0] == c) {
					return block;
				}
			}
		}
		return null;
	}
	public GameObject RefreshBlockAt(Vector3Int i) {
		char c = GetDataAt(i);
		BuildingBlock block = GetBlock (c);
		GameObject originalBlockObject = (block != null) ? block.blockObject : null;
		GameObject newBlockObject = null;
		if (GetObjectAt(i) != null) {
			Destroy (GetObjectAt(i));
		}
		if (originalBlockObject != null) {
			Vector3 p = new Vector3 (i.x * cubeSize.x, i.z * cubeSize.y, -i.y * cubeSize.z);
			p = transform.TransformPoint (p);
			newBlockObject = Instantiate (originalBlockObject, p, transform.rotation) as GameObject;
			newBlockObject.transform.SetParent (transform);
		}
		SetObjectAt(i, newBlockObject);
		return newBlockObject;
	}

	public void ExpandMap (Vector3Int expansion) {
		Vector3Int offsetAdjustment = Vector3Int.zero;
		if (expansion.x < 0) { offsetAdjustment.x += expansion.x; }
		if (expansion.y < 0) { offsetAdjustment.y += expansion.y; }
		if (expansion.z < 0) { offsetAdjustment.z += expansion.z; }
		Vector3Int additionalRange = new Vector3Int (Mathf.Abs (expansion.x), Mathf.Abs (expansion.y), Mathf.Abs (expansion.z));
		GameObject[,,] newAllBlockObjects = new GameObject[
			allBlockObjects.GetLength(0)+additionalRange.z,
			allBlockObjects.GetLength(1)+additionalRange.y,
			allBlockObjects.GetLength(2)+additionalRange.x];
		char[,,] newMutableData = new char[
			mutableData.GetLength(0)+additionalRange.z,
			mutableData.GetLength(1)+additionalRange.y,
			mutableData.GetLength(2)+additionalRange.x];
		Vector3Int i = Vector3Int.zero;
//		Vector3Int newLimits = new Vector3Int (newMutableData.GetLength(2), newMutableData.GetLength(1), newMutableData.GetLength(0));
//		Vector3Int currentLimits = new Vector3Int (mutableData.GetLength(2), mutableData.GetLength(1), mutableData.GetLength(0));
		for (i.z = 0; i.z < mutableData.GetLength (0); ++i.z) {
			for (i.y = 0; i.y < mutableData.GetLength (1); ++i.y) {
				for (i.x = 0; i.x < mutableData.GetLength (2); ++i.x) {
					Vector3Int j = i - offsetAdjustment;
//					Debug.Log (j + " set to " + i);
					newMutableData [j.z, j.y, j.x] = mutableData [i.z, i.y, i.x];
					newAllBlockObjects [j.z, j.y, j.x] = allBlockObjects [i.z, i.y, i.x];
				}
			}
		}
		mutableData = newMutableData;
		allBlockObjects = newAllBlockObjects;
		indexOffset += offsetAdjustment;
	}

	public bool IndexInBounds(Vector3Int index) {
		return index.x >= indexOffset.x && index.x < mutableData.GetLength(2)+indexOffset.x
			&& index.y >= indexOffset.y && index.y < mutableData.GetLength(1)+indexOffset.y
			&& index.z >= indexOffset.z && index.z < mutableData.GetLength(0)+indexOffset.z;
	}
	public Vector3Int IndexDistanceOutside(Vector3Int index) {
		Vector3Int o = Vector3Int.zero;
		if(index.x < indexOffset.x) o.x = index.x-indexOffset.x;
		else if(index.x >= mutableData.GetLength(2)+indexOffset.x) o.x = index.x+1 - (mutableData.GetLength(2)+indexOffset.x);
		if(index.y < indexOffset.y) o.y = index.y-indexOffset.y;
		else if(index.y >= mutableData.GetLength(1)+indexOffset.y) o.y = index.y+1 - (mutableData.GetLength(1)+indexOffset.y);
		if(index.z < indexOffset.z) o.z = index.z-indexOffset.z;
		else if(index.z >= mutableData.GetLength(0)+indexOffset.z) o.z = index.z+1 - (mutableData.GetLength(0)+indexOffset.z);
		return o;
	}
	public static bool IndexWithin(Vector3Int i, int rad) {
		return Mathf.Abs (i.x) <= rad && Mathf.Abs (i.y) <= rad && Mathf.Abs (i.z) <= rad;
	}
	public void SetBlock(Vector3Int index, char c) {
		SetDataAt (index, c);
		dirtyList.Add (index);
	}
	public void RemoveBlock(Vector3Int index) { SetBlock (index, (char)0); }
	public void RefreshMap(bool all) {
		if (allBlockObjects == null) {
			allBlockObjects = new GameObject[calculatedSize.z,calculatedSize.y,calculatedSize.x];
		}
		if (!all) {
			for (int i = dirtyList.Count-1; i >= 0; i--) {
				RefreshBlockAt (dirtyList [i]);
				dirtyList.RemoveAt (i);
			}
		} else {
			Vector3Int i = Vector3Int.zero;
			for (i.z = 0; i.z < mutableData.GetLength (0); i.z++) {
				for (i.y = 0; i.y < mutableData.GetLength (1); i.y++) {
					for (i.x = 0; i.x < mutableData.GetLength (2); i.x++) {
						RefreshBlockAt (i);
					}
				}
			}
		}
	}
	/// <summary>read the map data and calculate the correct size based on text data</summary>
	/// <returns>The map size.</returns>
	public Vector3Int CalculateMapSize() {
		int row = 0, col = 0, maxCol = 0, maxRow = 0, dep = 1;
		for (int index = 0; index < map.Length; index++) {
			char c = map [index];
			if (c == '\r') { continue; }
			if (c == '\n') {
				if (col == 0) {
					dep++; row = 0;
				} else {
					row++;
				}
				if (row > maxRow) {
					maxRow = row;
				}
				col = 0;
			} else {
				col++;
				if (col > maxCol) {
					maxCol = col;
				}
			}
		}
		return new Vector3Int (maxCol, maxRow, dep);
	}

	void GetBoundingCorners(ref Vector3[] corners) {
		if(corners == null || corners.Length != 8) { corners = new Vector3[8]; }
		if (map == null || map == "" || enabled == false)
			return;
		if (calculatedSize == Vector3Int.zero) {
			calculatedSize = CalculateMapSize ();
		}
		float w=(calculatedSize.x * cubeSize.x), h=(calculatedSize.y * cubeSize.y), d=(calculatedSize.z * cubeSize.z);
		Vector3 offset = cubeSize / -2;
		offset.z *= -1;
		offset = transform.TransformVector (offset);
		corners[0] = transform.position + offset; // lower top left
		corners[1] = transform.position + transform.right * w + offset; // lower top right
		corners[3] = transform.position - transform.forward * h + offset; // lower bottom left
		corners[2] = corners[3] + transform.right * w; // lower bottom right
		Vector3 u = transform.up * d;
		for (int i = 0; i < 4; i++) {
			corners [4 + i] = corners [i] + u;
		}
	}

	public static Vector3Int NOT_AN_INDEX = new Vector3Int (-65536, -65536, -65536);
	/// <summary>Gets the index being pointed at by the given Camera+Mouse combo, at the given height.</summary>
	/// <returns>The <see cref="UnityEngine.Vector3Int"/>, <see cref="GenerateMap.NOT_AN_INDEX"/> if nothing is hit.</returns>
	/// <param name="cam">Cam.</param>
	/// <param name="height">height of the plane.</param>
	public Vector3Int GetIndexBeingPointedAtHeight(Camera cam, float height) {
		if (cam != null) {
			Ray r = cam.ScreenPointToRay (Input.mousePosition);
			Vector3 p = transform.position;
			p.y += (height) * cubeSize.y;
			Plane plane = new Plane (transform.up, p);
			float whereHit = 0;
			if (plane.Raycast (r, out whereHit)) {
				Vector3 hit = r.origin + r.direction * whereHit;
				return TranslateWorldToCoordinate (hit);
			}
		}
		return NOT_AN_INDEX;
	}
	GameObject line_hit;
	/// <summary>Gets the index being pointed at by the given Camera+Mouse combo</summary>
	/// <returns>The <see cref="UnityEngine.Vector3Int"/>, <see cref="GenerateMap.NOT_AN_INDEX"/> if nothing is hit.</returns>
	/// <param name="cam">Cam.</param>
	/// <param name="face">returns which face is selected: {0, TOP, BOTTOM, FRONT, BACK, LEFT, RIGHT}</param>
	public Vector3Int GetIndexBeingPointedAt(Camera cam, out int face) {
		if (cam == null) { face = 0; return NOT_AN_INDEX; }
		return GetIndexBeingPointedAt (cam.ScreenPointToRay (Input.mousePosition), out face);
	}
	public Vector3Int GetIndexBeingPointedAt(Ray ray, out int face) {
		RaycastHit rh = new RaycastHit ();
		if (Physics.Raycast (ray, out rh)) {
			Vector3 p = rh.point -= rh.normal * 0.125f;
			NS.Lines.Make (ref line_hit, rh.point, rh.point + rh.normal, Color.red);
			Vector3Int index = TranslateWorldToCoordinate (p, out face);
			if (face == 0) {
				float nx = Vector3.Dot (transform.right, rh.normal);
				float ny = Vector3.Dot (transform.up, rh.normal);
				float nz = Vector3.Dot (transform.forward, rh.normal);
				float anx = Mathf.Abs (nx);
				float any = Mathf.Abs (ny);
				float anz = Mathf.Abs (nz);
				if(anx > any && anx > anz) { face = nx < 0 ? LEFT : RIGHT; }
				if(any > anx && any > anz) { face = ny < 0 ? BOTTOM : TOP; }
				if(anz > any && anz > anx) { face = nz < 0 ? BACK : FRONT; }
			}
			return index;
		}
		face = 0;
		return NOT_AN_INDEX;
	}
	public const int TOP = 1, BOTTOM = 2, FRONT = 4, BACK = 8, LEFT = 16, RIGHT = 32;
	/// <summary>Translates the world coordinate to map coordinate space</summary>
	/// <returns>The world to coordinate.</returns>
	/// <param name="worldPosition">World position.</param>
	/// <param name="face">returns which face is selected: {0, TOP, BOTTOM, FRONT, BACK, LEFT, RIGHT}</param>
	/// <param name="allowMultipleFaces">If set to <c>true</c> allow multiple faces with binary |.</param>
	public Vector3Int TranslateWorldToCoordinate(Vector3 worldPosition, out int face, bool allowMultipleFaces = false, 
		float faceThreshold = 0.45f) {
		Vector3 hit = transform.InverseTransformPoint (worldPosition);
		Vector3 index = new Vector3 (hit.x / cubeSize.x, hit.z / cubeSize.z, hit.y / cubeSize.y);
		Vector3Int finalResult = new Vector3Int((int)Mathf.Round(index.x), (int)Mathf.Round(index.y), (int)Mathf.Round(index.z));
		face = 0;
		index.x -= (int)finalResult.x;
		index.y -= (int)finalResult.y;
		index.z -= (int)finalResult.z;
		if(!allowMultipleFaces) {
			int faceIndex = 0;
			float[] faceWeight = new float[] {0,index.z,-index.z,index.y,-index.y,-index.x,index.x};
			for (int i = 1; i < 7; ++i) { if (faceWeight [i] > faceWeight [faceIndex]) { faceIndex = i; } }
			if (faceWeight [faceIndex] > faceThreshold) {
				face = 1 << (faceIndex - 1);
			}
		} else {
			if(index.z > faceThreshold)  face += TOP;
			if(index.z < -faceThreshold) face += BOTTOM;
			if(index.y > faceThreshold)  face += FRONT;
			if(index.y < -faceThreshold) face += BACK;
			if(index.x > faceThreshold)  face += RIGHT;
			if(index.x < -faceThreshold) face += LEFT;
		}
		finalResult.y *= -1;
		// TODO find out why a valid index is not given when grabbing from below...
//		if (index.z < 0) {
//			Debug.Log ("BELOW " + index + " " + finalResult+" "+face);
//		}
		return finalResult;
	}
	public Vector3Int TranslateWorldToCoordinate(Vector3 worldPosition) {
		Vector3 hit = transform.InverseTransformPoint (worldPosition);
		Vector3 index = new Vector3 (hit.x / cubeSize.x, hit.z / cubeSize.z, hit.y / cubeSize.y);
		return new Vector3Int((int)Mathf.Round(index.x), (int)-Mathf.Round(index.y), (int)Mathf.Round(index.z));
	}
	public Vector3 TranslateCoordinateToWorld(Vector3Int index) {
		Vector3 p = transform.position;
		p += transform.right * index.x * cubeSize.x;
		p += transform.forward * -index.y * cubeSize.z;
		p += transform.up * index.z * cubeSize.y;
		return p;
	}
	public Vector3Int AddFaceToIndex(Vector3Int index, int face) {
		if((face & TOP) != 0)    index.z += 1;
		if((face & BOTTOM) != 0) index.z -= 1;
		if((face & FRONT) != 0)  index.y -= 1;
		if((face & BACK) != 0)   index.y += 1;
		if((face & RIGHT) != 0)  index.x += 1;
		if((face & LEFT) != 0)   index.x -= 1;
		return index;
	}
	public Vector3 TranslateCoordinateToWorld(Vector3Int index, int face) {
		index = AddFaceToIndex (index, face);
		Vector3 p = transform.position;
		p += transform.right * index.x * cubeSize.x;
		p += transform.forward * -index.y * cubeSize.z;
		p += transform.up * index.z * cubeSize.y;
		return p;
	}
	GameObject line_select, line_create;
	public void HighlightFace(Vector3Int index, int face, ref GameObject line_select) {
		Vector3 w = TranslateCoordinateToWorld (index), half = transform.TransformVector(cubeSize / 2);
		Vector3 p = w + half; // upper top right
		Vector3[] line = new Vector3[5];
		Vector3 u = transform.up * cubeSize.y, r = transform.right * cubeSize.x, f = transform.forward * cubeSize.z;
		switch(face) {
		case TOP:
			line [0] = p            ;
			line [1] = p     - r    ;
			line [2] = p     - r - f;
			line [3] = p         - f; break;
		case BOTTOM:
			line [0] = p - u        ;
			line [1] = p - u - r    ;
			line [2] = p - u - r - f;
			line [3] = p - u     - f; break;
		case FRONT:
			line [0] = p            ;
			line [1] = p     - r    ;
			line [2] = p - u - r    ;
			line [3] = p - u        ; break;
		case BACK:
			line [0] = p         - f;
			line [1] = p     - r - f;
			line [2] = p - u - r - f;
			line [3] = p - u     - f; break;
		case RIGHT:
			line [0] = p            ;
			line [1] = p - u        ;
			line [2] = p - u     - f;
			line [3] = p         - f; break;
		case LEFT:
			line [0] = p     - r;
			line [1] = p - u - r;
			line [2] = p - u - r - f;
			line [3] = p     - r - f; break;
		}
		line[4] = line[0];
		NS.Lines.Make (ref line_select, line, 5, Color.red);
		line_select.SetActive (true);
	}
	void FixedUpdate() {
		if (!enableFreeEdit) return;
		RefreshMap (false);
	}

	private bool isInBounds = false, isExpanding = false;
	Vector3Int expansion = Vector3Int.zero;
	void Update () {
		if (!enableFreeEdit) return;
		float dx = Input.GetAxis ("Mouse X"), dy = Input.GetAxis ("Mouse Y");
		if (dx != 0 || dy != 0) { // if there is mouse motion
			int face;
			selectIndex = GetIndexBeingPointedAt(Camera.main, out face);
			createIndex = AddFaceToIndex (selectIndex, face);
//			if (createIndex.z < 0) {
//				createIndex = GetIndexBeingPointedAtHeight (Camera.main, -0.5f);
//				face = 0;
//				selectIndex = NOT_AN_INDEX;
//			}
			Color ofBox = Color.grey;
			if ((isInBounds = IndexInBounds(createIndex))) {
				ofBox = Color.green;
				isExpanding = false;
				expansion = Vector3Int.zero;
			} else if (autoExpand && (isExpanding = IndexWithin((expansion = IndexDistanceOutside(createIndex)), expansionBorderSize))) {
				ofBox = Color.cyan;
			}
			if (face != 0) {
				if (rightClickRemoves) {
					HighlightFace (selectIndex, face, ref line_select);
				}
				if (leftClickAdds != "") {
					NS.Lines.MakeBox (ref line_create, TranslateCoordinateToWorld (createIndex), cubeSize * 0.875f, transform.rotation, ofBox);
					line_create.SetActive (true);
				}
			} else {
				if (leftClickAdds != "") {
					if (line_select != null) line_select.SetActive (false);
					NS.Lines.MakeBox (ref line_create, TranslateCoordinateToWorld (selectIndex), cubeSize, transform.rotation, ofBox);
					line_create.SetActive (true);
				}
			}
		}
		if (leftClickAdds != "" && Input.GetMouseButtonDown (0)) {
			if (isExpanding) {
				ExpandMap (expansion);
				isInBounds = true;
			}
			if (isInBounds) {
//				Debug.Log ("Create Index: " + createIndex);
				SetBlock (createIndex, leftClickAdds[0]);
//				Debug.Log (ToText ('.'));
			}
		}
		if (rightClickRemoves && Input.GetMouseButtonDown (1)) {
			if (IndexInBounds (selectIndex)) {
				RemoveBlock (selectIndex);
//				Debug.Log (ToText ('.'));
			}
		}
	}
}