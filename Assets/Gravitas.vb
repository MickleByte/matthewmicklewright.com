
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'            Gravitas - Newtonian Gravity Simulator         '
'            Created Oct 2016 - Apr 2017                    '
'            By Matthew Micklewright                        '
'            For AQA A-level Computer Science coursework    '
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

Imports System.Threading
Imports System.IO



Public Class Canvas

    'global variables:
    Dim g As Graphics
    Dim planets(20) As planet ' stores all instances of the class planet
    Dim RunThread As Boolean = False ' represents whether the simulation is running or paused, true = running
    Dim MouseDown As Boolean = False ' used to show in the program if the mouse button is held down or not, True = down
    Dim ClickX As Integer ' used to show the X location of a click
    Dim ClickY As Integer ' used to show the Y location of a click
    Dim WhichCursor As String


    Sub sortByRadius()
        Dim Rad(20) As Integer
        Dim conversion(20) As planet
        For n = 0 To 20
            conversion(n) = New planet
        Next

        For index = 0 To 20
            Rad(index) = planets(index).Radius ' puts the masses into an array
        Next

        MergeSort(Rad) 'sorts the massess of the planets into ascending order

        'now that the radii have been ordered in an array they are matched to their planets again, when a planet is matched it is moved to a temporary array (conversion)
        'this means that the planets are re-ordered into the conversion array with ascending radii

        Dim index2 As Integer ' this stores the next available slot in the temporary conversion array
        For plnIndex = 0 To 20 ' for every value in the Mass array
            For index = 0 To 20  ' for every planet

                If Not ((planets(plnIndex).InUse = False) Or (Rad(index) = 0)) Then 'if the planet is in use in the current simulation
                    If Math.Round(planets(plnIndex).Radius) = Rad(index) Then ' if the mass of that planet matches the mass in the array
                        conversion(index2).Radius = planets(plnIndex).Radius ' puts all of the values into the conversion array so that the main array can be re-ordered
                        conversion(index2).Density = planets(plnIndex).Density
                        conversion(index2).Colour = planets(plnIndex).Colour
                        conversion(index2).Xposition = planets(plnIndex).Xposition
                        conversion(index2).Yposition = planets(plnIndex).Yposition
                        conversion(index2).InUse = planets(plnIndex).InUse
                        conversion(index2).InitialVelocity = planets(plnIndex).InitialVelocity
                        planets(plnIndex).InUse = False
                        index2 += 1
                    End If
                End If

            Next
        Next
        ClearPlanets()  ' now delete all of the planets from the planets array as they have been put into the temporary conversion array

        For index = 0 To 20 ' moves all values from conversion to planets so that it can be used in the rest of the program.
            If conversion(index).InUse = True Then
                planets(index).Radius = conversion(index).Radius
                planets(index).Density = conversion(index).Density
                planets(index).Colour = conversion(index).Colour
                planets(index).Xposition = conversion(index).Xposition
                planets(index).Yposition = conversion(index).Yposition
                planets(index).InUse = conversion(index).InUse
                planets(index).InitialVelocity = conversion(index).InitialVelocity
            End If
        Next
    End Sub ' orders the planets into descending order by radii in the array planets

    Public Sub MergeSort(ByVal ar() As Integer)
        DoMergeSort(ar, 0, ar.Length - 1)
    End Sub ' passes the radii to the merge sorting sub with minimum and maximum values

    Private Sub DoMergeSort(ByVal array() As Integer, ByVal Min As Integer, ByVal Max As Integer)
        'keeps down the array into halves and then quaters the sub arrays etc. - Divide stage
        ' then recombines the array in descending order  - Conquer Stage
        If Min >= Max Then
            Return
        End If
        Dim length As Integer = Max - Min + 1
        Dim middle As Integer = Math.Floor((Min + Max) / 2)
        DoMergeSort(array, Min, middle)
        DoMergeSort(array, middle + 1, Max)
        Dim temp(array.Length - 1) As Integer
        For i As Integer = 0 To length - 1
            temp(i) = array(Min + i)
        Next
        Dim m1 As Integer = 0
        Dim m2 As Integer = middle - Min + 1
        For i As Integer = 0 To length - 1
            If m2 <= Max - Min Then
                If m1 <= middle - Min Then
                    If temp(m1) > temp(m2) Then
                        array(i + Min) = temp(m2)
                        m2 += 1
                    Else
                        array(i + Min) = temp(m1)
                        m1 += 1
                    End If
                Else
                    array(i + Min) = temp(m2)
                    m2 += 1
                End If
            Else
                array(i + Min) = temp(m1)
                m1 += 1
            End If
        Next

    End Sub ' performs the recursive merge sort

    Sub loadSim(ByVal path As String)
        ClearPlanets() ' resets entire array
        Dim reader As StreamReader = New StreamReader(path) 'opens the text file in the location path
        Dim textLine As String = reader.ReadLine() ' textline stores a line of text read from the file that was opened in the above line
        Do Until textLine = Nothing ' = keep reading lines until you reach a blank line
            For Each planet In planets ' = go through every instance in the array
                If planet.InUse = False Then ' = if the inuse marker is false then the array location is empty so new data can be stored in it
                    'store the various details about the planet in the memory locations:
                    planet.Radius = textLine
                    planet.Density = reader.ReadLine()
                    planet.Colour = reader.ReadLine()
                    planet.Xposition = reader.ReadLine()
                    planet.Yposition = reader.ReadLine()
                    planet.InitialVelocity.Xcomp = reader.ReadLine()
                    planet.InitialVelocity.Ycomp = reader.ReadLine()

                    planet.InUse = True ' shows that this position in the array is now in use
                    Exit For ' can now leave for as a position has beeen found
                End If
            Next
            textLine = reader.ReadLine()
        Loop
        g.Clear(Color.Black)
        DrawGraphics() ' refresh graphics on screen to show the new simulation that has been loaded above
        reader.Close() ' close the text document
    End Sub ' loads simulations

    Sub SaveSim(ByVal Path As String)
        Dim writer As IO.StreamWriter = New IO.StreamWriter(Path) ' open file in specified location
        For Each planet In planets ' = go through every instance in the array
            If planet.InUse = True Then ' = if the inuse marker is true then there is a planet whos data has to be saved
                'output to the textfile all nessecary data on the planet: 
                writer.WriteLine(planet.Radius)
                writer.WriteLine(planet.Density)
                writer.WriteLine(planet.Colour)
                writer.WriteLine(planet.Xposition)
                writer.WriteLine(planet.Yposition)
                writer.WriteLine(planet.InitialVelocity.Xcomp)
                writer.WriteLine(planet.InitialVelocity.Ycomp)

            End If
        Next
        writer.Close() ' close the text file

    End Sub ' saves simulations

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.KeyPreview = True ' needed for keyboard shortcuts

        NumericUpDownSpeed.Text = 5

        Me.DoubleBuffered = True ' supposed to help with flickering graphics - I am still unsure whether it does or not
        SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        Me.SetStyle(ControlStyles.UserPaint, True)
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)


        ' hides textboxes that stores initial velocity - this is a feature that was going to be visible to users but it made more sense from a design point of view to take it away, however it's nessecary to the code.
        TextBoxInitialVelXComp.Hide()
        TextBoxInitialVelYComp.Hide()
        ' hides textboxes that stores X and Y locations in graphics - this is a feature that was going to be visible to users but it made more sense from a design point of view to take it away, however it's nessecary to the code.
        TextBoxXcor.Hide()
        TextBoxYcor.Hide()

        ButtonInfoTool.Enabled = False
        'starting data for the 3 textboxes - this gives user an idea of what sort of val they need to enter without having to look at tool tip
        TextBoxDensity.Text = 5
        TextBoxRadius.Text = 20
        ComboBoxColour.Text = "Grey"



        'window formating etc. :
        Panel1.Hide() ' hidden so that it can be shown again - this refreshes the panel so that it apears properly
        Me.WindowState = FormWindowState.Maximized ' maximises window
        Panel2.Dock = DockStyle.Right 'docks controls to right hand side of screen
        ' makes canvas the same size as screen
        PictureBoxCanvas.Width = (Me.Width)
        PictureBoxCanvas.Height = (Me.Height)

        g = Me.PictureBoxCanvas.CreateGraphics 'enables graphics on the canvas

        ClearPlanets() 'sets up the array of planets
        DrawGraphics()
        'makes sure there is a location for the program to open when the load or save buttons are pressed
        If (Not System.IO.Directory.Exists("C:\Gravitas")) Then
            System.IO.Directory.CreateDirectory("C:\Gravitas")
        End If

        TextBoxAdvRad.Enabled = False
        TextBoxAdvDen.Enabled = False
        TextBoxAdvInitialX.Enabled = False
        TextBoxAdvInitialY.Enabled = False
        TextBoxAdvMass.Enabled = False
        TextBoxAdvXpos.Enabled = False
        TextBoxAdvYpos.Enabled = False
    End Sub ' intial formating for the form and setting up graphics etc.

    Private Sub Form1_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        If e.Shift And e.KeyCode.ToString = "A" Then ' shift-A = add planet tool
            ButtonAddPlanet.PerformClick()
        End If

        If e.Shift And e.KeyCode.ToString = "D" Then ' shift-D = delete planet tool
            ButtonDeletePlanet.PerformClick()
        End If

        If e.Shift And e.KeyCode.ToString = "R" Then ' shift-R = Run simultion
            ButtonPlay.PerformClick()
        End If

        If e.Control And e.KeyCode.ToString = "S" Then ' ctrl-S = save simulation
            ButtonSave.PerformClick()
        End If

        If e.Control And e.KeyCode.ToString = "L" Then  ' ctrl-L = load simulation
            ButtonSave.PerformClick()
        End If
    End Sub   'handles keyboard Shortcuts

    Sub CreatePlanet()
        Dim strVel As String '  temporary storage for the output of the initial velocity textboxes, this is so that validation can take place

        For Each planet In planets ' = go through array
            If planet.InUse = False Then '  = until you find a position that is not currently in use

                'establishes values for radius etc. from textboxes
                planet.Radius = TextBoxRadius.Text
                planet.Density = TextBoxDensity.Text
                planet.Colour = ComboBoxColour.Text
                planet.Xposition = TextBoxXcor.Text
                planet.Yposition = TextBoxYcor.Text
                planet.InUse = True 'marks that planet as in use, therefore it won't be written over

                strVel = TextBoxInitialVelYComp.Text
                If TextBoxInitialVelYComp.Text = Nothing Then 'if the textbox is blank then taking it and directly putting it into an integer would cause it to crash
                    planet.InitialVelocity.Ycomp = 0
                Else
                    planet.InitialVelocity.Ycomp = TextBoxInitialVelYComp.Text
                End If

                strVel = TextBoxInitialVelXComp.Text
                If TextBoxInitialVelXComp.Text = Nothing Then 'if the textbox is blank then taking it and directly putting it into an integer would cause it to crash
                    planet.InitialVelocity.Xcomp = 0
                Else
                    planet.InitialVelocity.Xcomp = TextBoxInitialVelXComp.Text
                End If

                Exit For
            End If
        Next
    End Sub ' Gathers data for new entry into the planet array

    Sub DrawGraphics()
        If Not CheckBoxTrace.Checked Then
            g.Clear(Color.Black)
        End If
        For Each planet In planets ' = every instance of planet in the array
            If planet.InUse = True Then 'if its in use flag is true then it needs to be drawn    
                Dim x As Decimal = (planet.Xposition - 0.5 * (planet.Radius * 2))
                Dim y As Decimal = (planet.Yposition - 0.5 * (planet.Radius * 2))


                'there isn't a way to have one fill statement so this select case draws the planet with the appropriate colour
                Select Case planet.Colour
                    Case "Grey"
                        g.FillEllipse(Brushes.Gray, x, y, (planet.Radius * 2), (planet.Radius * 2))
                    Case "Blue"
                        g.FillEllipse(Brushes.Blue, x, y, (planet.Radius * 2), (planet.Radius * 2))
                    Case "Yellow"
                        g.FillEllipse(Brushes.Yellow, x, y, (planet.Radius * 2), (planet.Radius * 2))
                    Case "Red"
                        g.FillEllipse(Brushes.Red, x, y, (planet.Radius * 2), (planet.Radius * 2))
                    Case "Green"
                        g.FillEllipse(Brushes.Green, x, y, (planet.Radius * 2), ((planet.Radius * 2)))
                End Select


            End If
        Next


    End Sub 'updates the graphic output

    Private Sub DoWork()
        sortByRadius()
        While RunThread = True ' until the pause button is pressed runThread will be true, therefore this will keep looping until pause is pressed.
            DrawGraphics() ' update grapics
            'if the merge on collision checkbox is ticked then this boolean will be true and so the program will check to see whether any planets need to merge and merge them if they do
            If CheckBoxCollision.Checked = True Then
                checkMerge()
            End If
            calcForces() ' find new planet positions
        End While
    End Sub  ' checks pause button has not been pressed

    Sub checkMerge()
        Dim merge As Boolean = False ' will be true when two planets should merge
        For Each planet In planets ' = for every instance of planet in the array
            If planet.InUse <> False Then ' only checks the planets that are currently in use in the simulation
                ' will go through every planet in the array that is in use, OTHER than the current planet. This means that each planet gets compared to every other planet
                For Each pln In planets
                    If pln.InUse <> False And Not ((planet.Xposition = pln.Xposition) And (planet.Yposition = pln.Yposition) And (planet.Radius = pln.Radius)) Then

                        'if the planet is within the other planet in both the x and the y then merge will become true to show that they should merge together
                        If pln.Xposition > (planet.Xposition - planet.Radius) And pln.Xposition < (planet.Xposition + planet.Radius) And pln.Yposition > (planet.Yposition - planet.Radius) And pln.Yposition < (planet.Yposition + planet.Radius) Then
                            merge = True
                        End If


                        'merges the planets together if they should be
                        If merge = True Then
                            Dim M As Decimal
                            If planet.mass >= pln.mass Then ' makes sure that the smaller planet is added to the larger
                                planet.InitialVelocity.Xcomp = ((planet.InitialVelocity.Xcomp * planet.mass) + (pln.InitialVelocity.Xcomp * pln.mass)) / (planet.mass + pln.mass)
                                planet.InitialVelocity.Ycomp = ((planet.InitialVelocity.Ycomp * planet.mass) + (pln.InitialVelocity.Ycomp * pln.mass)) / (planet.mass + pln.mass)
                                planet.Radius += pln.Radius  'adds the radii together
                                M = (planet.mass + pln.mass)
                                planet.Density = M / ((4 / 3) * Math.PI * Math.Pow(planet.Radius, 3))


                                'planet.Density = (planet.mass + pln.mass) / ((4 / 3) * Math.PI * (Math.Pow((planet.Radius + pln.Radius), 3))) ' finds density by mass / volume
                                ' finds intial velocity by finding the momentum's and adding them


                                'removes other planet from simulation
                                pln.InUse = False
                                pln.Radius = Nothing
                                pln.Density = Nothing
                                pln.Xposition = Nothing
                                pln.Yposition = Nothing

                            ElseIf planet.mass < pln.mass Then ' makes sure that the smaller planet is added to the larger
                                pln.InitialVelocity.Xcomp = ((planet.InitialVelocity.Xcomp * planet.mass) + (pln.InitialVelocity.Xcomp * pln.mass)) / (planet.mass + pln.mass)
                                pln.InitialVelocity.Ycomp = ((planet.InitialVelocity.Ycomp * planet.mass) + (pln.InitialVelocity.Ycomp * pln.mass)) / (planet.mass + pln.mass)
                                pln.Radius += planet.Radius  'adds the radii together
                                M = (planet.mass + pln.mass)
                                pln.Density = M / ((4 / 3) * Math.PI * Math.Pow(pln.Radius, 3))

                                'removes other planet from simulation
                                planet.InUse = False
                                planet.Radius = Nothing
                                planet.Density = Nothing
                                planet.Yposition = Nothing
                                planet.Yposition = Nothing

                            End If
                            merge = False ' resets merge for later checks
                        End If
                    End If
                Next
            End If
        Next
    End Sub ' checks to see whether there are any planets that need to be merged into one and if so it does this

    Sub calcForces()


        Dim distance As Decimal ' stores a distance between two planets
        Dim force As Decimal  ' stores the force between two planets
        Dim ForceHor As Decimal ' stores the horizontal component of the force between two planets
        Dim ForceVer As Decimal  ' stores the vertical component of the force between two planets
        Dim GravConst As Decimal = 0.00667 ' gravitational constant - changed for my model so that it works in an interesting way in the scale that I have


        Dim index1 As Integer
        Dim index2 As Integer

        For index1 = 0 To 20

            For index2 = 0 To 20
                If (planets(index1).InUse = True) And (planets(index2).InUse = True) And index1 < index2 Then

                    distance = Math.Sqrt(Math.Pow((planets(index1).Xposition - planets(index2).Xposition), 2) + Math.Pow((planets(index1).Yposition - planets(index2).Yposition), 2))
                    force = planets(index1).mass * planets(index2).mass
                    force = force * GravConst
                    force = force / Math.Pow(distance, 2)
                    ForceHor = force * ((planets(index1).Xposition - planets(index2).Xposition) / distance)
                    ForceVer = Math.Sqrt(Math.Abs(Math.Pow(force, 2) - Math.Pow(ForceHor, 2)))



                    'for planets(index1)
                    Dim tempXval As Decimal = ForceHor / (2 * planets(index1).mass)
                    Dim tempYval As Decimal = ForceVer / (2 * planets(index1).mass)

                    If planets(index1).Xposition > planets(index2).Xposition Then
                        tempXval = Math.Abs(tempXval) * -1
                    Else
                        tempXval = Math.Abs(tempXval)
                    End If
                    If planets(index1).Yposition > planets(index2).Yposition Then
                        tempYval = Math.Abs(tempYval) * -1
                    Else
                        tempYval = Math.Abs(tempYval)
                    End If

                    planets(index1).InitialVelocity.Xcomp += tempXval
                    planets(index1).InitialVelocity.Ycomp += tempYval


                    'for planets(index2)
                    tempXval = ForceHor / (2 * planets(index2).mass)
                    tempYval = ForceVer / (2 * planets(index2).mass)

                    If planets(index2).Xposition > planets(index1).Xposition Then
                        tempXval = Math.Abs(tempXval) * -1
                    Else
                        tempXval = Math.Abs(tempXval)
                    End If
                    If planets(index2).Yposition > planets(index1).Yposition Then
                        tempYval = Math.Abs(tempYval) * -1
                    Else
                        tempYval = Math.Abs(tempYval)
                    End If

                    planets(index2).InitialVelocity.Xcomp += tempXval
                    planets(index2).InitialVelocity.Ycomp += tempYval

                End If
            Next

            planets(index1).Xposition += (planets(index1).InitialVelocity.Xcomp / 50)
            planets(index1).Yposition += (planets(index1).InitialVelocity.Ycomp / 50)
        Next


        CalcSimSpeed() ' goes to sub to calculate the waiting period between calculations and therefore the frame rate/ speed of simulation

    End Sub ' calculates the movement of all of the planets and updates the positions of the planets accordingly

    Sub CalcSimSpeed()

        Dim speed As String = NumericUpDownSpeed.Text ' gets speed value from the nummeric up down which has a range 0 to 10

        'the variable speed represents the number of milliseconds that the program will wait before calculating the next frame of the animation

        If Empty(speed) = True Then ' checks to see that the speed is not nothing
            speed = 50 ' if so the speed is set to 50
        Else
            If IsInt(speed) = True Then ' makes sure the speed is castable as an integer
                speed = CInt(speed)
                If speed = 0 Then ' is the speed is 0 then the program cannot wait 0 milliseconds because the flicker would be so bad that the program would be unusable
                    speed = 1 ' instead the wait time is set to 1 millisecond which is still bad but managable
                Else
                    speed = speed * 10 ' if the speed is not 0 then it is multiplied by ten as there is no noticable difference between 1,2,3 etc. but there is between 20, 30, 50 etc.
                End If
            Else ' if it is not it may be a negative number etc.
                speed = 50 ' sets speed to 50
                speed = CInt(speed) ' casts speed to an integer
            End If

        End If

        Threading.Thread.Sleep(speed) ' waits the amount of time calculated above.
    End Sub ' used to workout the appropriate speed for the model to run at based on input from numericUpDownSpeed. Also has validation for this input

    Function Empty(ByVal Value As String)
        If Value = Nothing Then
            Return True
        Else
            Return False
        End If
    End Function 'function used to determine whether the given string is null or has a value, True if Null, False if Not Null.

    Function IsInt(ByVal val As String)
        Dim val1 As Integer
        Dim length As Integer = val.Length
        Dim Invalid As Boolean = False


        For n = 0 To (length - 1) ' for every character in the string
            val1 = Asc(val(n)) ' get the ascii value of the character
            If Not (val1 >= 48 And val1 <= 57) Then ' if the ascii value is between 48 and 57 then the character is a number
                Return False 'if not then the whole value is not a number and will cause an error when it is cast as an integer so false is returned
                MsgBox("False")
            End If
        Next

        If val = Nothing Then ' incase the value is nothing and length = 0
            Return True ' the integer value will end up being 0 
        End If

        Return True ' if it reaches this stage and it has not returned false then it must be a number so true is returned

    End Function 'checks to see if the givin value is an integer - used for validation

    Sub ClearPlanets()
        Dim index As Integer
        ' goes through positions 0 to 20 and puts in a new instance of planet
        For index = 0 To 20
            planets(index) = New planet ' 
        Next
    End Sub ' sets up the planet array as a series of 20 instances of planet all without any values, this is used both at the start of the program to create the array and at any point that all planets must be deleted

    Sub PutInAdv(ByVal position As Integer)
        'Puts in :

        TextBoxAdvRad.Text = planets(position).Radius ' Radius
        TextBoxAdvDen.Text = planets(position).Density 'Density
        TextBoxAdvInitialX.Text = planets(position).InitialVelocity.Xcomp ' Initial Velocity X component
        TextBoxAdvInitialY.Text = planets(position).InitialVelocity.Ycomp ' Initial Velocity Y component
        TextBoxAdvXpos.Text = planets(position).Xposition ' Planet Y position
        TextBoxAdvYpos.Text = planets(position).Yposition ' Planet X position
        TextBoxAdvMass.Text = planets(position).mass 'Planet Mass

    End Sub ' puts the appropriate values into the advanced tab for the required planet

    Sub DrawOneObj(ByVal col As String, ByVal diameter As Integer, ByVal x As Integer, ByVal y As Integer)
        x -= 0.5 * diameter ' re-adjusts the X and Y locations as the regular draw ellipse will not put the centre right on the click location 
        y -= 0.5 * diameter

        Select Case col ' draws the circle of the chosen colour and size in the click location
            Case "Grey"
                g.FillEllipse(Brushes.Gray, x, y, diameter, diameter)
            Case "Blue"
                g.FillEllipse(Brushes.Blue, x, y, diameter, diameter)
            Case "Yellow"
                g.FillEllipse(Brushes.Yellow, x, y, diameter, diameter)
            Case "Red"
                g.FillEllipse(Brushes.Red, x, y, diameter, diameter)
            Case "Green"
                g.FillEllipse(Brushes.Green, x, y, diameter, diameter)
        End Select
    End Sub ' draws just one planet when given: location (x,y) diameter and colour

    Function ValidateInput(ByVal Str As String, ByVal field As String, ByVal min As Integer, ByVal max As Integer) ' min and max are the upper and lower bounds of excepted values, field is the name of the input (e.g. Radius), Str is the input
        If Str = Nothing Then ' checks that it is not null
            MsgBox("You Must Enter A {0} Before You Create A Planet!", field)
        Else
            If IsInt(Str) = True Then ' goes to function that ensures it is an integer
                If CInt(Str) < min Or CInt(Str) > max Then ' checks range
                    MsgBox(field & " must be integer between " & min & " and " & max)
                Else
                    Return True ' if it meets all requirements then it can return true
                End If
            Else
                MsgBox(field & " must be integer between " & min & " and " & max)
            End If
        End If
    End Function ' makes sure that a input is a valid integer - true if valid, false if invalid

    Private Sub PictureBoxCanvas_mousedown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBoxCanvas.MouseDown
        TextBoxXcor.Text = e.X ' gets the X and Y location of the click and stores it in the textboxes so the user can see it numericaly
        TextBoxYcor.Text = e.Y
        ' A big list of nested ifs that decide what needs to happen when the user clicks down on the canvas:

        If RunThread = False Then ' if the simulation is NOT running then
            TextBoxAdvRad.Text = "" ' clear all textboxes
            TextBoxAdvDen.Text = ""
            TextBoxAdvInitialX.Text = ""
            TextBoxAdvInitialY.Text = ""
            TextBoxAdvXpos.Text = ""
            TextBoxAdvYpos.Text = ""
            TextBoxAdvMass.Text = ""
            If CheckBoxTrace.Checked Then ' clear the canvas if the trace planets tool is on
                g.Clear(Color.Black)
            End If
            DrawGraphics()
            If (WhichCursor = "A") Then ' If the user wants to create a planet
                Dim inputStr As String
                Dim field As String
                Dim rangeMin As String
                Dim rangeMax As String
                inputStr = TextBoxRadius.Text
                field = "Radius"
                rangeMin = 1
                rangeMax = 120
                If ValidateInput(inputStr, field, rangeMin, rangeMax) Then ' validates Radius
                    inputStr = TextBoxDensity.Text
                    field = "Density"
                    rangeMin = 1
                    rangeMax = 1000
                    If ValidateInput(inputStr, field, rangeMin, rangeMax) Then ' validates Density
                        MouseDown = True ' sets this variable to true so that the program knows the mouse button is currently being held down
                        ClickX = e.X ' gets the X and Y location of the click
                        ClickY = e.Y
                        'works out details for the advanced tab and puts them in for the user to see
                        TextBoxAdvDen.Text = TextBoxDensity.Text ' outputs the new planets density
                        TextBoxAdvRad.Text = TextBoxRadius.Text ' outputs the new planets radius
                        TextBoxAdvMass.Text = ((4 / 3) * (Math.PI) * Math.Pow((TextBoxRadius.Text), 3)) * (TextBoxDensity.Text) ' outputs the new planets mass
                        TextBoxAdvXpos.Text = e.X ' outputs the new planets X and Y position
                        TextBoxAdvYpos.Text = e.Y
                        Dim col As String = ComboBoxColour.Text ' gets colour from combo box
                        Dim diameter As Integer = (TextBoxRadius.Text * 2) ' finds the diameter as double the radius
                        Dim x As Integer = TextBoxXcor.Text ' gets the X and Y location of the click
                        Dim y As Integer = TextBoxYcor.Text
                        DrawOneObj(col, diameter, x, y)
                    End If
                End If
            ElseIf WhichCursor = "D" Then 'If user is trying to delete a planet
                Dim index As Integer = CheckClick(e.X, e.Y) ' function that checks to see if a click is within a planet - returns that planets number OR 23 if no planet is selected
                If index <> 23 Then
                    planets(index).InUse = False
                    planets(index).Radius = Nothing
                    planets(index).Density = Nothing
                    planets(index).Colour = Nothing
                    planets(index).InitialVelocity.Xcomp = Nothing
                    planets(index).InitialVelocity.Ycomp = Nothing
                    planets(index).TotalVelocity.Xcomp = Nothing
                    planets(index).TotalVelocity.Ycomp = Nothing
                    g.Clear(Color.Black)
                    DrawGraphics() ' update graphics so that user no longer sees the deleted planet on screen
                    MouseDown = False ' mousedown is now false
                End If
            ElseIf WhichCursor = "I" And (CheckBoxToggleAdvanced.Checked = True) Then 'if neither tool is in use then it selects the planet and displays its data in the advanced tab as the information tool must be in use
                Dim index As Integer = CheckClick(e.X, e.Y) ' function that checks to see if click is within a planet - returns that planets number OR 23 if no planet is selected
                If index = 23 Then
                    DrawGraphics()
                    TextBoxAdvRad.Text = ""
                    TextBoxAdvDen.Text = ""
                    TextBoxAdvInitialX.Text = ""
                    TextBoxAdvInitialY.Text = ""
                    TextBoxAdvXpos.Text = ""
                    TextBoxAdvYpos.Text = ""
                    TextBoxAdvMass.Text = ""
                Else
                    planets(index).SelectForAdvanced = True
                    PutInAdv(index)
                    g.Clear(Color.Black)
                    DrawGraphics()
                    'draws a white circle larger than the planet then redraws the planet on top
                    'this gives the impression that there is a white circle around the planet showing it as selected
                    Dim d As Integer = planets(index).Radius * 2 ' finds diameter
                    g.FillEllipse(Brushes.White, (planets(index).Xposition - planets(index).Radius - 5), (planets(index).Yposition - planets(index).Radius - 5), (d + 10), (d + 10)) ' draws the white circle - canvas does not have to be cleared because the only changes made are hidden by this larger white circle
                    ' then draws the regular planet on top
                    DrawOneObj(planets(index).Colour, (planets(index).Radius * 2), planets(index).Xposition, planets(index).Yposition)
                End If
            End If
        End If

    End Sub ' essentialy a big list of nested ifs that decide what needs to happen when the user clicks down on the canvas

    Function CheckClick(ByVal x As Integer, ByVal y As Integer)
        Dim index As Integer = 0
        For index = 0 To 20 ' for every planet in array
            If planets(index).InUse = True Then ' where it is currently being used in the model
                If (x > (planets(index).Xposition - planets(index).Radius)) And (x < (planets(index).Xposition + planets(index).Radius)) Then ' if the x coordinate is between the planets upper and lower radius bound
                    If (y > (planets(index).Yposition - planets(index).Radius)) And (y < (planets(index).Yposition + planets(index).Radius)) Then ' and if the y is between the upper and lower bound for that planet
                        Return index ' return the number planet in the array
                    End If
                End If
            End If
        Next
        Return 23 ' if not any of the planets in the array it returns a value greater than 20 to show that it could not be found
    End Function ' looks through every planet trying to find a planet that contains the passed coordinates

    Private Sub PictureBoxCanvas_mousemove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBoxCanvas.MouseMove
        TextBoxMouseXcor.Text = e.X ' updates mouse location
        TextBoxMouseYcor.Text = e.Y
        If WhichCursor = "A" And (MouseDown = True) Then ' if the user just created a planet and is still holding the mouse down this means they are drawing a line to give it an intial velocity
            ' DrawGraphics() ' updates graphics
            Dim yCord, xCord As String ' gets the X and Y location for centre of planet
            xCord = TextBoxXcor.Text
            yCord = TextBoxYcor.Text
            g.Clear(Color.Black)
            DrawGraphics()
            g.DrawLine(Pens.Green, CInt(xCord), CInt(yCord), CInt(e.X), CInt(e.Y)) 'draws green line from centre of planet to cursor location
            ' draws the main planet again
            Dim col As String = ComboBoxColour.Text ' gets colour
            Dim diameter As Integer = (TextBoxRadius.Text * 2) ' gets diameter
            Dim x As Integer = TextBoxXcor.Text ' gets centre
            Dim y As Integer = TextBoxYcor.Text
            DrawOneObj(col, diameter, x, y)
            Me.TextBoxInitialVelXComp.Text = (ClickX - e.X) ' stores the intial velocity in the textbox based on the length and direction of the line that has been drawn
            Me.TextBoxInitialVelYComp.Text = (ClickY - e.Y)
            Me.TextBoxAdvInitialX.Text = (ClickX - e.X)
            Me.TextBoxAdvInitialY.Text = (ClickY - e.Y)
        End If
    End Sub ' sorts out what to do when the mouse is move on the canvas

    Private Sub PictureBoxCanvas_mouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBoxCanvas.MouseUp

        If WhichCursor = "A" And (MouseDown = True) Then ' if the user has created a planet and is now releasing there mouse click
            MouseDown = False 'shows that the mouse click is now over

            CreatePlanet()  ' creates a planet where the user clicked
            If CheckBoxTrace.Checked Then
                g.Clear(Color.Black)
            End If
            DrawGraphics()  ' updates graphics which will now include the new planet

            TextBoxInitialVelXComp.Clear() ' clears out all of the textboxes so that the next planet's data is not corrupted by leftover data from this planet
            TextBoxInitialVelYComp.Clear()
            TextBoxXcor.Clear()
            TextBoxYcor.Clear()
            TextBoxAdvDen.Clear()
            TextBoxAdvRad.Clear()
            TextBoxAdvXpos.Clear()
            TextBoxAdvYpos.Clear()
            TextBoxAdvInitialX.Clear()
            TextBoxAdvInitialY.Clear()
            TextBoxAdvMass.Clear()

        End If


    End Sub ' sorts out what happens when the user releases the click on the canvas

    Private Sub ButtonClearAll_Click(sender As Object, e As EventArgs) Handles ButtonClearAll.Click
        If RunThread = True Then
            RunThread = False ' stops the model while the user reads the dialog below
            Dim result As Integer = MessageBox.Show("Do you want to delete your entire model? Any unsaved changes will be lost", "Are you sure?", MessageBoxButtons.YesNo) ' brings up an 'are you sure' box

            If Not result = DialogResult.No Then ' if the user clicks continue

                ButtonAddPlanet.Enabled = True ' enables all of the buttons that were disabled whilst the simulation was running
                ButtonLoad.Enabled = True
                ButtonSave.Enabled = True
                ButtonPlay.Text = "Play" ' and resets the pause button to a play button as the simulation has now ended
                ButtonDeletePlanet.Enabled = True

                ClearPlanets() ' deletes every planet in the array
                g.Clear(Color.Black)
                ButtonInfoTool.PerformClick()
            Else
                RunThread = True ' restarts the model if the user does selects no on the message box
                Dim MainThread As Thread = New Thread(AddressOf DoWork) ' puts the work of running the simulation in a new parralel thread as the program needs to be animating the screen as well as waiting for the pause button click event at the same time - this requires two parralel processing threads
                MainThread.IsBackground = True
                MainThread.Start()
            End If
        Else
            Dim result As Integer = MessageBox.Show("Do you want to delete your entire model? Any unsaved changes will be lost", "Are you sure?", MessageBoxButtons.YesNo) ' brings up an 'are you sure' box
            If Not result = DialogResult.No Then ' if the user clicks continue

                ClearPlanets() ' deletes every planet in the array
                g.Clear(Color.Black)
                ButtonInfoTool.PerformClick()
            End If
        End If

    End Sub ' stops the array and deletes all of the planets that were in it

    Private Sub ButtonPlay_Click(sender As Object, e As EventArgs) Handles ButtonPlay.Click

        If ButtonPlay.Text = "Play" Then ' if the simulation was previously paused and the user now wants it to run:

            ButtonPlay.Text = "Pause" ' changes the interface to make more sense for a running simulation by disabling certain buttons and therefore tools
            ButtonAddPlanet.Enabled = False
            ButtonDeletePlanet.Enabled = False
            ButtonInfoTool.Enabled = False
            ButtonSave.Enabled = False
            ButtonLoad.Enabled = False

            TextBoxAdvDen.Clear() ' clears out all of the textboxes in the advanced tab
            TextBoxAdvInitialX.Clear()
            TextBoxAdvInitialY.Clear()
            TextBoxAdvRad.Clear()
            TextBoxAdvXpos.Clear()
            TextBoxAdvYpos.Clear()
            TextBoxAdvMass.Clear()

            g.Clear(Color.Black)

            RunThread = True ' sets the boolean that represents if the simulation is running to true
            WhichCursor = "NONE" ' disables the tools that effect clicking on the canvas

            Cursor = Cursors.Arrow ' resets cursor to and arrow to demonstrate to the user that they no longer are using the tool that they had previously selected

            Dim MainThread As Thread = New Thread(AddressOf DoWork) ' puts the work of running the simulation in a new parralel thread as the program needs to be animating the screen as well as waiting for the pause button click event at the same time - this requires two parralel processing threads
            MainThread.IsBackground = True
            MainThread.Start()

        Else ' pauses the simulation
            'resets UI to paused mode by re-enabling tools etc. :
            ButtonPlay.Text = "Play"
            ButtonAddPlanet.Enabled = True
            ButtonDeletePlanet.Enabled = True
            ButtonLoad.Enabled = True
            ButtonSave.Enabled = True
            ButtonInfoTool.Enabled = True

            RunThread = False ' marks the simulation as paused
            DrawGraphics() ' updates the graphics
            ButtonInfoTool.PerformClick()
        End If

    End Sub ' sorts out what to do when the play/pause button is pressed

    Private Sub ButtonAddPlanet_Click(sender As Object, e As EventArgs) Handles ButtonAddPlanet.Click
        Cursor = Cursors.Cross ' changes the cursor type to a cross so that the user can visualy see what tool they have selected
        WhichCursor = "A" ' sets the delete tool and info tool booleans to false as they are not in use and the add tool to true as this is in use
        

        ButtonDeletePlanet.Enabled = True ' changes the buttons to disable the tool that is currently selected - this way the user can easily see which tool they are using and what their other options are
        ButtonAddPlanet.Enabled = False
        ButtonInfoTool.Enabled = True
    End Sub ' sets up the add planet tool

    Private Sub ButtonDeletePlanet_Click(sender As Object, e As EventArgs) Handles ButtonDeletePlanet.Click
        Cursor = Cursors.Hand ' changes the cursor type to a cross so that the user can visualy see what tool they have selected
        WhichCursor = "D" ' sets the add tool and info tool booleans to false as they are not in use and the delete tool to true as this is in use
  

        ButtonDeletePlanet.Enabled = False ' changes the buttons to disable the tool that is currently selected - this way the user can easily see which tool they are using and what their other options are
        ButtonAddPlanet.Enabled = True
        ButtonInfoTool.Enabled = True
    End Sub ' sets up the delete planet tool

    Private Sub ButtonInfoTool_Click(sender As Object, e As EventArgs) Handles ButtonInfoTool.Click
        Cursor = Cursors.Arrow ' changes the cursor type to a cross so that the user can visualy see what tool they have selected
        WhichCursor = "I" ' sets the add tool and delete tool booleans to false as they are not in use and the info tool to true as this is in use


        ButtonDeletePlanet.Enabled = True ' changes the buttons to disable the tool that is currently selected - this way the user can easily see which tool they are using and what their other options are
        ButtonAddPlanet.Enabled = True
        ButtonInfoTool.Enabled = False
    End Sub ' sets up the information tool

    Private Sub ButtonSave_Click(sender As Object, e As EventArgs) Handles ButtonSave.Click
        'sets up the save file dialogue
        With SaveFileDialog1
            .FileName = "" ' file name is inputed by user into dialogue later
            .Title = "Save Simulation" ' title of window
            .InitialDirectory = "C:\Gravitas" ' start location of the file explorer navigation
            .Filter = "Text files|*.txt" ' text files only
        End With
        SaveFileDialog1.ShowDialog() ' shows dialogue
        If SaveFileDialog1.FileName <> "" Then ' makes sure the user puts in a name for the save
            SaveSim(SaveFileDialog1.FileName) ' goes to sub which saves the file
        End If

    End Sub ' sorts out the save file dialogue comming up and what it says to the user

    Private Sub ButtonLoad_Click(sender As Object, e As EventArgs) Handles ButtonLoad.Click
        'sets up the load file dialogue
        With OpenFileDialog1
            .FileName = "" ' file name is inputed by user into dialogue so for now it is blank
            .Title = "Load Simulation" ' title of window
            .InitialDirectory = "C:\Gravitas" ' start location of the file explorer navigation
            .Filter = "Text files|*.txt" ' text files only
            .ShowDialog() ' shows dialogue to user
        End With
        Dim path As String = OpenFileDialog1.FileName ' Gets file name
        If OpenFileDialog1.FileName <> "" Then ' if it's not Null
            loadSim(path) ' it is passed to the load sub which sorts out loading the simulation into the program
        End If
    End Sub ' sorts out the load file dialogue

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxToggleAdvanced.CheckedChanged
        If Not CheckBoxToggleAdvanced.Checked Then ' user wants to hide AdvTab
            Panel1.Hide() ' hide advanced tab
            If RunThread = False Then
                DrawGraphics()
            End If
            ButtonAddPlanet.PerformClick() ' selects the add planet tool so that the user is not still using the info tool which they cannot now see
        Else ' user wants advTab to show
            Panel1.Show()
            ButtonInfoTool.PerformClick() ' selects the info tool as this is how the 
        End If
    End Sub 'Sorts out whether the advanced tab needs to be showing when the user changes the show advanced checkbox

End Class



Class planet
    Structure displacement
        Public Xcomp As Decimal
        Public Ycomp As Decimal
        Public InUse As Boolean
    End Structure

    Private _radius As Decimal
    Private _density As Decimal
    Private _colour As String
    Private _xposistion As Decimal
    Private _yposistion As Decimal
    Private _inuse As Boolean

    Public SelectForAdvanced As Boolean
    Public InitialVelocity As displacement
    Public TotalVelocity As displacement

    Public Property Radius As Decimal
        Set(value As Decimal)
            _radius = value
        End Set
        Get
            Return _radius
        End Get
    End Property

    Public Property Density As Decimal
        Set(value As Decimal)
            _density = value
        End Set
        Get
            Return _density
        End Get
    End Property

    Public Property Colour As String
        Set(value As String)
            _colour = value
        End Set
        Get
            Return _colour
        End Get
    End Property

    Public Property Xposition As Decimal
        Set(value As Decimal)
            _xposistion = value
        End Set
        Get
            Return _xposistion
        End Get
    End Property

    Public Property Yposition As Decimal
        Set(value As Decimal)
            _yposistion = value
        End Set
        Get
            Return _yposistion
        End Get
    End Property

    Public Property InUse As Boolean
        Set(value As Boolean)
            _inuse = value
        End Set
        Get
            Return _inuse
        End Get
    End Property


    Function volume()
        Dim vol As Decimal
        vol = Math.Pow(_radius, 3)
        vol = vol * Math.PI
        vol = vol * (4 / 3)
        Return vol
    End Function

    Function mass()
        Dim mas As Decimal
        mas = volume() * _density
        Return mas
    End Function

End Class
