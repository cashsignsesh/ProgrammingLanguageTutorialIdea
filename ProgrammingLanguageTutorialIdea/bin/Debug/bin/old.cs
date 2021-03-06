this.interactions.Reverse();
			this.pushedValues.Reverse();
			foreach (Tuple<OrderItem,OrderItem,OrderMathType>tpl in this.interactions) {
				if (tpl.Item1.type==OrderItemType.PARSED) {
					
					// mov to eax
					UInt16 espOffset=(UInt16)(pushedValues.IndexOf(tpl.Item1)*4);
					if (espOffset<=SByte.MaxValue)
						sender.addBytes(new Byte[]{0x8B,0x44,0x24,(Byte)(espOffset)});//MOV EAX,[ESP+-OFFSET]
					else
						;//UNDONE::
					
				}
				else {
					
					sender.pushValue(tpl.Item1.unparsedValue);
					sender.addByte(0x58); //POP EAX
					
				}
				
				if (tpl.Item3== OrderMathType.ADDITION) {
					
					if (tpl.Item2.type==OrderItemType.PARSED) {
						
						UInt16 espOffset=(UInt16)(pushedValues.IndexOf(tpl.Item2)*4);
						if (espOffset<=SByte.MaxValue)
							sender.addBytes(new Byte[]{1,0x44,0x24,(Byte)(espOffset)}); //ADD [ESP+-OFFSET],EAX
						else
							;//UNDONE::
							
					}
					else {
						
						sender.pushValue(tpl.Item2.unparsedValue);
						sender.addBytes(new Byte[]{1,4,0x24}); //ADD [ESP],EAX
						sender.addBytes(new Byte[]{1,0x44,0x24,4}); //ADD [ESP+4],EAX
						
					}
					
				}
				else /*==OrderMathType.SUBTRACTION */ {
					
					if (tpl.Item2.type==OrderItemType.PARSED) {
						
						UInt16 espOffset=(UInt16)(pushedValues.IndexOf(tpl.Item2)*4);
						if (espOffset<=SByte.MaxValue)
							sender.addBytes(new Byte[]{0x29,0x44,0x24,(Byte)(espOffset)}); //SUB [ESP+-OFFSET],EAX
						else
							;//UNDONE::
							
					}
					else {
						
						sender.pushValue(tpl.Item2.unparsedValue);
						sender.addBytes(new Byte[]{0x29,4,0x24}); //SUB [ESP],EAX
						sender.addBytes(new Byte[]{0x29,0x44,0x24,4}); //SUB [ESP+4],EAX
					
					}
					
				}
				
			}
			
			sender.addByte(0x58);//POP EAX
			
			UInt16 res=(UInt16)(pushedValues.Count*4);
			
			if (res<=SByte.MaxValue)
				sender.addBytes(new Byte[]{0x83,0xC4,(Byte)res}); //SUB ESP,SBYTE
			else {
				
				sender.addBytes(new Byte[]{0x81,0xC4}); //SUB ESP,DWORD
				sender.addBytes(BitConverter.GetBytes((UInt32)res));
				
			}
			
			sender.addByte(0x50);//PUSH EAX