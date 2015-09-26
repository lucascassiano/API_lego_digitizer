//Positive Connectors
int p0 = 8;
int p1 = 9;
int p2 = 10;
int p3 = 11;

//Negative Connectors
int n0 = 5;
int n1 = 4;
int n2 = 3;
int n3 = 2;

//Input Pins
int i0 = A0;
int i1 = A1;
int i2 = A2;
int i3 = A3;


//input grid values
int inputGrid[4][4] = {
 {0,0,0,0},
 {0,0,0,0},
 {0,0,0,0},
 {0,0,0,0}
};


void setup() {
  // put your setup code here, to run once:
  pinMode(p0, OUTPUT);
  pinMode(p1, OUTPUT);
  pinMode(p2, OUTPUT);
  pinMode(p3, OUTPUT);
  
  pinMode(n0, OUTPUT);
  pinMode(n1, OUTPUT);
  pinMode(n2, OUTPUT);
  pinMode(n3, OUTPUT);
  
  pinMode(i3, INPUT);
  pinMode(i2, INPUT);
  pinMode(i1, INPUT);
  pinMode(i0, INPUT);
  
  Serial.begin(9600);
  
  //Setting all pins
  digitalWrite(n0,LOW);
  digitalWrite(n1,LOW);
  digitalWrite(n2,LOW);
  digitalWrite(n3,LOW);
  
  digitalWrite(p0,HIGH);
  digitalWrite(p1,HIGH);
  digitalWrite(p2,HIGH);
  digitalWrite(p3,HIGH);
  
  //Populating the matrix
  for(int i=0; i<4; i++){ 
    for(int j=0; j<4; j++){
      inputGrid[i][j] = 0;
    }
  }
  
}

void loop() {
  //Reading Each Point
  for(int i=0; i<4; i++){ 
    for(int j=0; j<4; j++){
      inputGrid[i][j] = ReadPosition(i,j);
      delay(10);
    }
  }
  
  Serial.println("-#-"); //Represents a new reading
  Serial.println(String(inputGrid[0][0])+","+String(inputGrid[1][0])+","+String(inputGrid[2][0])+","+String(inputGrid[3][0]));
  Serial.println(String(inputGrid[0][1])+","+String(inputGrid[1][1])+","+String(inputGrid[2][1])+","+String(inputGrid[3][1]));
  Serial.println(String(inputGrid[0][2])+","+String(inputGrid[1][2])+","+String(inputGrid[2][2])+","+String(inputGrid[3][2]));
  Serial.println(String(inputGrid[0][3])+","+String(inputGrid[1][3])+","+String(inputGrid[2][3])+","+String(inputGrid[3][3]));
  Serial.flush(); 
  
  //delay(100);
  digitalWrite(13,HIGH);
  //delay(400);
  //digitalWrite(13,LOW);
}

int GetResistenceFromValue(int value){
  if(value>10) 
  {
    float Vin= 5;
    float Vout= 0;
    float R1= 1000; //1k ohm
    int R2= 0;
    float buffer= 0;
    buffer = value * Vin;
    Vout = (buffer)/1024.0;
    buffer= (Vin/Vout) -1;
    R2= R1 * buffer;
    if(R2>0)
      return (int)R2;
    else return 0;
  }
  else 
    return 0;
}

int ReadPosition(int i, int j){
  if(i ==0){
     pinMode(n0,OUTPUT);
     digitalWrite(n0,LOW);
     pinMode(n1,INPUT);//this goes to high-impedance
     pinMode(n2,INPUT);
     pinMode(n3,INPUT);
  }
  
  else if(i ==1){
    pinMode(n0,INPUT);  
    pinMode(n1,OUTPUT);   
    digitalWrite(n1,LOW);
    pinMode(n2,INPUT); 
    pinMode(n3,INPUT);
}
  
  else if(i==2){
    pinMode(n0,INPUT);
    pinMode(n1,INPUT); 
    pinMode(n2,OUTPUT); 
    digitalWrite(n2,LOW);
    pinMode(n3,INPUT);
  }
  
  else if(i==3){
    pinMode(n0,INPUT);
    pinMode(n1,INPUT); 
    pinMode(n2,INPUT);
    pinMode(n3,OUTPUT); 
    digitalWrite(n3,LOW);
  }
  
  //Positive connectors
  if(j ==0){
   pinMode(p0,OUTPUT);
   digitalWrite(p0,HIGH); 
   pinMode(p1,INPUT);
   pinMode(p2,INPUT);
   pinMode(p3,INPUT);  
  }
  
  else if(j ==1){
    pinMode(p0,INPUT); 
    pinMode(p1,OUTPUT);
    digitalWrite(p1,HIGH); 
    pinMode(p2,INPUT);
    pinMode(p3,INPUT);
  }
  
  else if(j==2){
    pinMode(p0,INPUT);
    pinMode(p1,INPUT);
    pinMode(p2,OUTPUT);
    digitalWrite(p2,HIGH);
    pinMode(p3,INPUT);        
  }
  
  
  else if(j==3){
    pinMode(p0,INPUT);
    pinMode(p1,INPUT);
    pinMode(p2,INPUT);
    pinMode(p3,OUTPUT);
    digitalWrite(p3,HIGH);    
  }
  
  int value = 0;
  if(i ==0){
    value = GetResistenceFromValue(analogRead(i0));
  }
  
  else if(i ==1){
    value = GetResistenceFromValue(analogRead(i1));
  }
  
  else if(i==2){
    value = GetResistenceFromValue(analogRead(i2));
  }
  
  else if(i==3){
    value = GetResistenceFromValue(analogRead(i3));
  }
  
  return value;
}

