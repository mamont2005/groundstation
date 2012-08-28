
int inByte = 0;         // incoming serial byte

void setup()
{
  Serial.begin(115200);
}

unsigned long time = 0;
void loop()
{
  //if (millis() > time + 20)
  {
    int a0 = analogRead(A0);
    int a1 = analogRead(A1);
    Serial.print("g ");
    Serial.print(a0 / 1024.f, 6);
    Serial.print(" ");
    Serial.println(a1 / 1024.f, 6);
    time = millis();
  }
   
  /* 
  if (Serial.available() > 0) 
  {
    inByte = Serial.read();
    Serial.print(inByte, BYTE);
  }
  */
}



