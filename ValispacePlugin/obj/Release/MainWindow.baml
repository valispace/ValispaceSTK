   M S B A M L   `   `   `  ���� J  FValispacePlugin, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:ValispacePluginValispacePlugin       ValispacePlugin.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     /localclr-namespace:ValispacePlugin     $	��677��$��1113.9��$��Stretch=�$��Stretch=�.��� $��	0,0,22,25q�.+��� -     ��Name$  ProjectLabel��$��Right=�$��Bottom=�$��0,0,41.6,612.8q�$��22��o� -    o�Name$ MainMenu��$��Right=�$��25��$	��Top=�$��2q�$��	0,0,844,0q�$	��258������ .R�A�  A� Rows$ 1��$	��250��.@�m� $��   _Options   ��.@�m� -   $��Project Settings��m� -   $��Save Data Bindings��m� $��Preferences��m� $�� _DataSets ��.@�m� -   $��Manage DataSets��m� $��   _Action   ��.@�m� $��Refresh��m� $��   _Help   ��.@�m� $��Version���� $
��Left=�$	��630��$��0,30,0,0q�$	��Top=�$
��1079�����	#FFEDEDED.@��� $��
Attributes��$��-2,-2,-53.4,0q�����  �� EndPoint$	 0,1&� �� 
StartPoint$	 0,0&�.���  � Color$ 	#FFF0F0F0�� � Offset$ 0Q�� $ Silver��$ 1Q�.��� -    �Name$ StackPanelInput��$��0,-8,-3,-16q����	#FFE0DEDE.+��� $
��Left=�$��62��$��467,526,0,0q�
�������$	��Top=�$	��242���� $
��Left=�$��62��$��207,526,0,0q�
�������$	��Top=�$	��239��]� -    ]�Name$ InputGroup2��$��10,46,11,28.8q�.+�&   System.Windows.Controls.DataGrid  -   	  Name$	 STKDataXAML��$	��458��$	��775��
 %� IsVirtualizing
 .  $
��Left=�$��Center=�$��0,10,0,79.2q� � 	Resources    AttributeRowContextMenu m�Name=  7System.Windows.Controls.Primitives.DataGridColumnHeader �� 
TargetType �� Property �� Value)  #System.Windows.Controls.DataGridRow �� Triggers �� Binding �� Value*  $System.Windows.Controls.DataGridCell��%�  &	        '  m     '  �     7   '  H    '�� L    �� .@�m� $ BindingReassign��-	   $��Assign/Reassign��m� $ 	RowDelete��-
   $��Delete Binding���� "  .���  � ��
 �������  � ��$ 1,1,1,1q���  � ��
 �   ���  � ��$ 2q���  � ��$
 Bold"��� "  .���  �@�ContextMenu8   -    ��  �� *isBold+ 1.a���  � ��
 ���p���  �� *isBold+ 2.a���  � ��
 �������  �� *isBold+ 3.a���  � ��
 �������  �� *isBold+ 4.a���  � ��
 �������  �� *isBold+ 5.a���  � ��
 ������� "   ��  �� *isBold+ 0.a���  � ��$ 0,0,0,0q���  �� *isBold+ 1.a���  � ��$ 0,0,0,0q���  � ��$
 Bold"���  �� *isBold+ 2.a���  � ��$ 10,0,0,0q���  � ��$ DemiBold"���  �� *isBold+ 3.a���  � ��$ 15,0,0,0q���  � ��$ DemiBold"���  �� *isBold+ 4.a���  � ��$ 20,0,0,0q���  � ��$ Medium"���  �� *isBold+ 5.a���  � ��$ 20,0,0,0q���  � ��$ Normal"��� " �� ��  �� *isBold+ 1.a���  � {� .  ��  �� *isBold+ 2.a���  � {� .  ��  �� *isBold+ 3.a���  � {� .  ��  �� *isBold+ 4.a���  � {� .  ��  �� *isBold+ 5.a���  � {� .     Columns 4  .System.Windows.Controls.DataGridTemplateColumn     CellTemplate �� .R���  ��Name$ CheckBoxSelect��0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Header$  STK Objects/Attributes��   
Foreground
 �   �   Width5  /System.Windows.Controls.DataGridLengthConverter$
 0.5*    
IsReadOnly . 1	  +System.Windows.Controls.DataGridBoundColumn 	  Binding �� *	VarName+  -   ValueColumn��$    Value��
 �   �$
 0.6*  .  �� *
VarValue+  $  Unit��
 �   �$
 0.3*  .  �� *	VarUnit+  -   ValiName��$ Binded Valis��
 �   �$
 0.7*  .   -   	ValiValue��$    Value��
 �   �$
 0.5*  .   -   ValiUnit��$
 Unit��
 �   �$
 0.3*  . � $��	0,0,0,-13q�$	��305��.+�R� -    R�Name$ projectTree��$��15,13,26,107q�
�������  �� DataType
   ValispacePlugin.Component �� ItemsSource �� Orientation   ValispacePlugin.Vali��%p  (��       *,
 +)(�� �     *, +)�� " 
  �� *Items+.R��� {�.  .+��� $ 
Horizontal=�.+��� $��Resources/component.png��$��5,0,5,0q�$��15��$��15��$	��Top=��� $��12(�$
��Bold"����� *Name+�� "  .R��� $ 
Horizontal=�.+��� $��Resources/vali.png��$��5,0,5,0q�$��10��$��10��$	��Top=��� 
����  ����� *Name+���� " Q�.�-   �� $��1q�
���   �$��8,483,26,10q�._��� -   $  	StatusBox��$
��Wrap=�$�� �����	#FFDCDBDB�� $��Status:��$
��Left=�$��	8,457,0,0q�$	��Top=��� -    ��Name$ RefreshButton��$��Refresh Valispace Data��$
��Left=�$��Bottom=�$	��134��$��20��$��797,0,0,574.8q��� $ ��Valispace Project Browser:��$
��Left=�$��	792,0,0,0q�$	��Top=�$	��144��$��27���� -   $��Update Valispace Project��$
��Left=�$	��Top=�$	��142��$��288,557,0,0q�$��25���� -   $��Update STK Scenario ��$
��Left=�$	��Top=�$	��142��$��567,558,0,0q�$��25���� -   $ SelectValiButton��{�.  $��Select��$
��Left=�$��980,472,0,0q�$	��Top=�$��76���� -   $ CancelSelectButton��{�.  $��Cancel��$
��Left=�$��881,472,0,0q�$	��Top=�$��76���� $+��%Update Selected from Valispace to STK��$
��Left=�$��467,528,0,0q�$	��Top=�$	��205��$
��10.5(��� $+��%Update Selected from STK to Valispace��$
��Left=�$��207,528,0,0q�$	��Top=�$	��206��$��11(��� -   $  LoadProject_Msg��$b��\Please Load a project from Project Settings 
or press Refresh to load the existing project.��$	��Top=�$��52��$	��262��$��Italic$�$��816,283,11,0q��� $ Vertical=�$��5,537,900,38q�.+��� $ 
Horizontal=�$��5q�.+��� $��27��$��10,0,0,0q�
��� ����� $��	20,0,40,0q�$
��Wrap=�$��Values not in sync��$	��125��$��11(��� $ 
Horizontal=�$��5q�.+��� $��27��$��10,0,0,0q�
���\\���� $��	20,0,40,0q�$
��Wrap=�$��Incompatible Units��$	��125��$��11(��� $��Binding Color Status��$
��Left=�$��	8,518,0,0q�$	��Top=��� -   $ 
RefreshSTK��$��Refresh STK Data��$
��Left=�$��Bottom=�$	��146��$��20��$��633,0,0,574.8q��� -   $��SelectUnsync��$
��Left=�$	��Top=�$	��136��$��	17,31,0,0q��� $��Datasets��$��52,-1,-113.2,-0.8q����� $	 0,1&�$	 0,0&�.��� $ 	#FFF0F0F0��$ 0Q�� $ 	#FFCECDCD��$ 1Q�.��� $
��Left=�$	��483��$	��Top=�$	��902��$��0,20,0,0q�.+�R� -   $��DPTree��{�.  $
��Left=�$	��350��$	��Top=�$	��228��$��	10,24,0,0q� ��%�   'Q�       �� " Q�.���  � �� �� .R��� $ 
Horizontal=�.+��� $��Resources/component.png��$��5,0,5,0q�$��15��$��15��$	��Top=��� $��12(�$��SemiBold"����� ���� " Q�.�-   �� $��STK Data Providers��$
��Left=�$��	10,-7,0,0q�$	��Top=�$	��135���� -   $��STKDP_Count��$
��Left=�$��	121,5,0,0q�$	��Top=�$	��137��$��19��R� -   $ valiTree_XAML��{�.  $
��Left=�$	��350��$	��Top=�$	��229��$��
252,24,0,0q� ��%t   'Q�       �� " Q�.���  � �� �� .R��� -   $ 
Horizontal=�.+��� $��12(�$��SemiBold"����� �� $��Valispace Datasets��$
��Left=�$��
263,-8,0,0q�$	��Top=�$	��135��� -    $ MidPanel_Grid��$��52,53,78,-28q�.+��� -!     ��Name$  
STKDP_Col1��{�.  ! �� 
IsReadOnly! . $
��Left=�$��23��$��
32,391,0,0q�" �� TextWrapping$
" Wrap=�$��Time (default)��$	��Top=�$	��110���� $��	Column 1:��$
��Left=�$��-34,391,0,0q�$	��Top=�$��62���� -"   $  
STKDP_Col2��{�.  ! . $
��Left=�$��23��$��
32,418,0,0q�$
" Wrap=�$��	Select...��$	��Top=�$	��110���� $��	Column 2:��$
��Left=�$��-34,416,0,0q�$	��Top=�$��62���� $��Choose STK DataSet:��$
��Left=�$��-44,366,0,0q�$	��Top=�$	��135���� -#   $	��...��$
��Left=�$��148,418,0,0q�$	��Top=�$��20��$��22��	� -$   $��Use Object Time��# d� 	IsChecked$
# TrueY�$
��Left=�$��213,343,0,0q�$	��Top=�$	��116��	� -%   $��Specify Interval��$# FalseY�$
��Left=�$��213,366,0,0q�$	��Top=��� -&   $��StartTimeBoxXAML��! . $
��Left=�{�.  $��23��$��235,396,0,0q�$
" Wrap=�$��Scenario.StartTime��$	��Top=�$	��186���� $��Start:��$
��Left=�$��193,393,0,0q�$	��Top=�$��62���� -'   $��StopTimeBoxXAML��! . $
��Left=�{�.  $��23��$��235,424,0,0q�$
" Wrap=�$��Scenario.StopTime��$	��Top=�$	��186���� $��Stop:��$
��Left=�$��195,424,0,0q�$	��Top=�$��62���� -(   $��StepBoxXAML��! .  $
��Left=�$��23��$��369,361,0,0q�$
" Wrap=�$��60��$	��Top=�$��52���� $��Step (sec):��$
��Left=�$��361,337,0,0q�$	��Top=�$��66��$ J� RenderTransformOrigin$$ 0.5,0.5&��� $��Choose Valispace DataSet��$
��Left=�$��-44,445,0,-13q�$	��Top=�$	��172���� -)   $	��...��$
��Left=�$��148,476,0,-39q�$	��Top=�$��20��$��21���� -*   $��valispace_DataSet��! . $
��Left=�$��23��$��-28,476,0,-41q�$
" Wrap=�$	��Top=�$	��170���� -+   $��Bind Datasets��$
��Left=�$��213,491,0,-53q�$	��Top=�$	��199���� $��Choose STK Object:��$
��Left=�$��-44,319,0,0q�$	��Top=�$	��135���� -,   $��ChildrenDropdown��$
��Left=�$	��Top=�$	��170��$��-28,346,0,0q�$��20������ *ChildrenNameSTK+  --   $	 DSBindingGridXAML��$	��501��$	��554��%   AutoGenerateColumns% .  
 .  $
��Left=�$��Center=�$��508,10,-160,-28q�   DatasetRowContextMenu��%  '        &	 �     �� "  .���  � ��
 �������  � ��$ 1,1,1,1q���  � ��
 �   ���  � ��$ 2q���  � ��$
 Bold"��� .@�m� $ RowContDelete��-.   $��Delete Binding��m� $ RowContMenuProp��-/   $��
Properties��&   RowStyle& �� "  .���  �@�ContextMenu# [ ��  � ��$ 0q�    �� .R��� $ CheckBoxSelect��  $ 
Binding ID��
 �   �$
 0.3*  .  �� *bindID+  -0   
DSCol1��$  Column1��
 �   �$
 0.5*  .  �� *
StkDP_Col1+  -1   
DSCol2��$  Column2��
 �   �$
 0.5*  .  �� *DP2_FullName+  -2   ValiDataset��$ Valispace Dataset��
 �   �$
 0.7*  .  �� *valiDataset+�� -3   $ GenerateUpdateBtn��$��Generate and Update��$
��Left=�$��890,516,-160,-55q�$	��Top=�$	��172��$��22��