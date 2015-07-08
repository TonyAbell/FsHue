
Color Picker
https://github.com/vanderlee/colorpicker
http://bgrins.github.io/spectrum/
https://github.com/josedvq/colpick-jQuery-Color-Picker/
http://mjolnic.com/bootstrap-colorpicker/
https://github.com/buberdds/angular-bootstrap-colorpicker
https://github.com/ruhley/angular-color-picker
https://github.com/istvan-ujjmeszaros/jquery-colorpickersliders

public class Fruit
{
    int Weight { get ; set;}
}
public Interface IPrint {
  void Print();
}
public Interface IShow {
  void Show();
}
Apple : Fruit, IPrint
Banna : Fruit, IPrint
Berry : Fruit, IPrint

Veg
Squash : Veg, IPrint
Zuk: Veg, IPrint

void Print( IPrint item) {
  item.Print();
}



void Print(Apple fruit) {}
void Print(Banana fruit) {}
void Print(Berry fruit) {}
