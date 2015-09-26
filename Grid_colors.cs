using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEditor;



public class Grid_colors: TUI
{
	
	
	// When added to an object, draws colored rays from the
	// transform position.
	
	public int[,] grid;
	public int[,] _grid;
	private int gridVerticalSize = 26;
	
	public GameObject blockModel;
	public List<Block> blockList = new List<Block>(); //Blocks that will be used
	public GameObject[,,] createdBlocks;
	
	public List<int>blockIds = new List<int>();
	
	public GameObject[] blockModels;
	
	public int[,,] gridId = new int[4, 4, 12];
	
	private bool gridChanged = false;
	private Vector2 changePos = new Vector2(0,0);
	private int curGridValue = 0;
	private int prevGridValue = 0;
	private List<Vector4> gridChanges = new List<Vector4>();
	
	public bool debugMode = false;
	public int[,] debugGrid = new int[4,4];
	
	public bool drawOpenGLLines = true;
	public bool simulateObjects = true;
	
	void Start(){
		//Populating the grids values;
		grid = new int[gridSize, gridSize];
		_grid = new int[gridSize, gridSize];
		createdBlocks = new GameObject[gridSize, gridSize, gridVerticalSize];
		gridId = new int[gridSize, gridSize, gridVerticalSize];
		
		/*Those are the values for each cube association, you can add it manually changing the blockIds list*/
		blockIds.Add (0);
		blockIds.Add (10000); //white
		blockIds.Add (16000); //blue
		blockIds.Add (28257); //green
		blockIds.Add (23400); //yellow
		blockIds.Add (2300); //red

		
		blockIds.Add (0);
		
		for (int i=0; i<gridSize; i++)
			for (int j=0; j<gridSize; j++)
				for (int k=0; k<12; k++)
					gridId [i, j, k] = -1;
		
		if (!debugMode) {
			//open Serial connection
			this.Open ();
		}
	}
	
	void Update(){
		/*
		a00.text = grid [0, 0].ToString ();
		a01.text = grid [0, 1].ToString ();
		a02.text = grid [0, 2].ToString ();
		a10.text = grid [1, 0].ToString ();
		a11.text = grid [1, 1].ToString ();
		a12.text = grid [1, 2].ToString ();
		a20.text = grid [2, 0].ToString ();
		a21.text = grid [2, 1].ToString ();
		a22.text = grid [2, 2].ToString ();
		*/
		if (gridChanged) {
			OnGridChanged();
			gridChanged = false;
		}
		
		
		//Camera Rotation
		//Camera.main.gameObject.transform.RotateAround (point,Vector3.right,20 * Time.deltaTime * 0.1f);
		//transform.RotateAround (point,new Vector3(0.0f,1.0f,0.0f),20 * Time.deltaTime * speedMod);
	}
	
	public void ChangeGrid(int[,] newGrid){
		for (int i=0; i<gridSize; i++) {
			for(int j=0; j<gridSize; j++){
				ClearColumn(i,j);
				for(int z=0; z<newGrid[i,j]; z++)
				{
					gridId[i,j,z] = newGrid[i,j]-1; //Populating
					if(simulateObjects)
						AddBlock(0,i,j,z);
				}
			}
		}
	}
	
	
	void OnGridChanged(){
		
		foreach (Vector4 change in gridChanges) {
			ClearColumn((int)change.x,(int)change.y);
			for(int i=0; i<blockIds.Count; i++){
				if(change.w == blockIds[i] && change.w!=0)
				{
					//PopulateColumn((int)change.x,(int)change.y,i);
					if(i<blockModels.Length)
						AddBlock(i,(int) change.x, (int) change.y, 0);
					
				}
			}
			
		}
		
		gridChanges = new List<Vector4> ();
		gridChanged = false;
	}
	
	protected override void OnReceiveData (int [,] dataGrid)
	{
		for (int i=0; i<gridSize; i++) {
			for (int j=0; j<gridSize; j++) {
				int number = dataGrid[i,j];
				int closest = GetClosestNumber(blockIds,number);
				grid[i,j] = closest;
				
				//grid[i,j] = dataGrid[i,j];
			}
		}
		
		if (!gridChanged) {
			gridChanges.Clear();
			for (int i=0; i<gridSize; i++) {
				for (int j=0; j<gridSize; j++) {
					if (grid [i, j] != _grid [i, j] && !gridChanged) {
						gridChanges.Add(new Vector4(i,j,_grid [i, j],grid [i, j]));			
					}
				}
			}
			
			if(gridChanges.Count>0){
				gridChanged=true;
			}
		}
		
		//Save the data for next interaction
		for (int i=0; i<gridSize; i++) {
			for (int j=0; j<gridSize; j++) {
				_grid[i,j] = grid[i,j];
			}
		}
		
	}
	
	private void ClearColumn(int x, int y){
		for (int z=0; z<gridVerticalSize; z++) {
			RemoveBlock(x,y,z);
		}
	}
	
	private void PopulateColumn(int x, int y,int max){
		for (int z=0; z<max; z++) {
			if(simulateObjects)
				AddBlock(0,x,y,z);
			else
				gridId[x,y,z] = 0;
		}
	}
	
	
	//Adding block
	private void AddBlock(float x, float y, float z){ AddBlock ((int) x, (int) y, (int) z);}
	private void AddBlock(int x, int y, int z){
		//The lego block (2x2) has 40u x 40 x 24, so the height is 60% of the width and depth, ;
		createdBlocks [(int)x, (int)y,z] = (GameObject) Instantiate (blockModel, new Vector3 (x, 0.6f*z, y), Quaternion.identity);
		
	}
	
	//Adding a specific block
	private void AddBlock(int btype, float x, float y, float z){ AddBlock (btype, (int) x, (int) y, (int) z);}
	private void AddBlock(int btype,int x, int y, int z){
		//if (btype <= blockModels.Length)
		createdBlocks [(int)x, (int)y,z] = (GameObject) Instantiate (blockModels[btype], new Vector3 (x-gridSize*0.5f+0.5f, 0.6f*z, y-gridSize*0.5f+0.5f), Quaternion.identity);
		gridId [x, y, z] = btype;
	}
	//Remove Block
	private void RemoveBlock(float x, float y, float z){ RemoveBlock((int) x, (int) y, (int) z);}
	private void RemoveBlock(int x, int y, int z){
		if(createdBlocks[x, y, z]){
			GameObject destroyed = createdBlocks[x,y,z];
			createdBlocks[x,y,z]=null;
			Destroy(destroyed);
			gridId [x, y, z] = -1;
		}
	}
	
	private bool hasBlock(float x, float y, float z){
		return hasBlock ((int)x, (int)y, (int)z);
	}
	private bool hasBlock(int x, int y, int z){
		if (x >= 0 && x < gridSize && y >= 0 && y < gridSize && z >= 0 && z < gridVerticalSize) {
			if (createdBlocks [x, y, z] != null)
				return true;
			else
				return false;
		}
		else
			return false;
	}
	/*------------------------
	 *Line Drawing 
	 *------------------------*/
	public int gridSize = 100;
	public int squareSize = 10;
	public Material lineMaterial;
	public Color lineColor = Color.white;
	
	// Will be called after all regular rendering is done
	public void OnRenderObject ()
	{
		if (drawOpenGLLines) 
		{
			// Apply the line material
			lineMaterial.SetPass (0);
			
			GL.PushMatrix ();
			GL.MultMatrix (transform.localToWorldMatrix);
			// Set transformation matrix for drawing to
			// match our transform
			GL.MultMatrix (transform.localToWorldMatrix);
			// Vertex colors 
			GL.Color (lineColor);
			// Draw lines
			GL.Begin (GL.LINES);
			//Lines on X Axis
			for (int i = 0; i < gridSize+1; ++i) {
				// One vertex at transform position
				GL.Vertex3 (i * squareSize - gridSize * squareSize * 0.5f, -0.5f, -gridSize * squareSize * 0.5f);
				// Another vertex at edge of circle
				GL.Vertex3 (i * squareSize - gridSize * squareSize * 0.5f, -0.5f, gridSize * squareSize * 0.5f);
				
				//Z Axis
				// One vertex at transform position
				GL.Vertex3 (-gridSize * squareSize * 0.5f, -0.5f, i * squareSize - gridSize * squareSize * 0.5f);
				// Another vertex at edge of circle
				GL.Vertex3 (gridSize * squareSize * 0.5f, -0.5f, i * squareSize - gridSize * squareSize * 0.5f);
			}
			GL.End ();
			GL.PopMatrix ();
		}
	}
	/// <summary>
	/// Gets the grid as a voxel environment.
	/// </summary>
	/// <returns>The grid.</returns>
	public int[,,] getGrid(){
		return gridId;
	}
	/// <summary>
	/// Gets the vertical number of cubes on a given position.
	/// </summary>
	/// <returns>The height on position.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public int getHeightOnPosition(int x, int y){
		int size = 0;
		for(int i=0; i<gridVerticalSize; i++){
			size += gridId[x,y,i]+1;
		}
		return size;
	}
	
}
