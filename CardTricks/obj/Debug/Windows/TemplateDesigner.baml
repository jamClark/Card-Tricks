   M S B A M L   `   `   `  ����E  ACardTricks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:CardTricks.ModelsCardTricks.Models  5   	   'clr-namespace:CardTricks
CardTricks  5   	   7 clr-namespace:CardTricks.WindowsCardTricks.Windows  5   	   9!clr-namespace:CardTricks.ControlsCardTricks.Controls  5   	   )    #CardTricks.Windows.TemplateDesigner   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   2Modelsclr-namespace:CardTricks.Models     5   	   *localclr-namespace:CardTricks     5   	   2ctwin clr-namespace:CardTricks.Windows     5   	   3ctcon!clr-namespace:CardTricks.Controls     5   	   -   5          �Name$  windowTemplateDesigner��5	   	    � Title$ TemplateDesigner��5
   	   $��567.2��6"   $��999.2��61   $
��Auto=�5   	   $
��Auto=�5   	    � DataContext 5   	   ��  �� RelativeSource �� *Self+ � 	Resources 5      +   %CardTricks.Windows.InchToDpiConverter   InchConverter��%   &	          5   
   6#   5      .��5      � ���	#FF85A0B8 � ColumnDefinitions 5   
   �� 5      $��171.2 �6   $��50��6-   �� 5      $��20��6   �� 5      $	��300 �6   $��25��5       � RowDefinitions 5   
   �� 5      $	��46* �6   �� 5      $
��493* �6   .+�5    
   o� $��20��6   $	��Top=�6   $��	0,0,0.2,0q�63   $��3��6F   .@�5!      m� $
��File��6   .@�5"      m� $��New Template��6    m� Command 61   �� *NewTemplateCmd+�� 5#      m� 5$      $��Open Template��6   {�.  62   m� 5%      $��Save Template��6   {�.  62   5&      m� 5'      $
��Edit��6   .@�5(      m� $��
Unlock All��6    6/   �� *UnlockElementsCmd+5)      5*      �� 5-   
   -   5         ��Name$ scrollviewerWorkspace��5-      $��5,9.6,0.6,10.2q�66   $��1��6N   $��1��6[   $
��Auto=�6k   $
��Auto=�6�   .��5.      �� -   5        	 ��Name$	 canvasWorkspace��5.      ��5/      �� 50      
 �� EndPoint$
 0.5,1&�6*    �� 
StartPoint$ 0.5,0&�6;   .��51      �  � Color$ 	#FF677991��6'    � Offset$ 0.007Q�69   � 52      $ 	#FFA0B1C7��6'   $ 1Q�69   53      55      56      �� 59   
   -   5         ��Name$  itemscontrolPropertyEditor��59      $��2��5:      $��5.4,9.6,10.2,10.2q�5;      $��1��5<      ���	#3FE8E8E8$
��Auto=�5>   H   $
��Auto=�6z   ��6   ��  �� Path$ EditorPanelControls�5B      �� 5E   
   $��2��6   $
��Left=�6'   $��1.4,9.6,0,10.2q�6B   $��5��6Z   $��1��6d   ���	#FF85A0B86q   �� 5H   
   -   5         ��Name$ lisviewTemplateList��5H      $��10,9.6,0.6,10.2q�5K      $��1��5L      ��5I      �� $ Game.ObservableTemplates� �� UpdateSourceTrigger$ PropertyChanged=���5M      �� 5N       �� 
TargetType" ��6   .�5O      -   5        5Q      ��5T      �� 5U      .R�5V      �� .+�5W      �� $��2q�6:   ��6$   �� *Name+6:   5X      5Y      5[      �� 5\   
   $
��Left=�6   $��0,9.6,0,10.2q�62   $��1��6H   $��5��6U   ���	#FF85A0B8$��1��6v   5]      5^      