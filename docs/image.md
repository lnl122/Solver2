В ходе решения различных заданий будет необходимо распознавать картинки на изображении
Изображение - графический файл, полученный как задание
Картинка - отдельный объект для распознавания

## виды исходных данных
- картинки могут быть представлены поштучно: одна картинка - одно изображение
- картинки могут быть представлены слитно: несколько картинок на одном изображении
- изображения могут находится внутри скриптов (в браузере красиво убираются с экрана, но при парсинге страницы с заданием необходимо это учитывать)
- расположение картинок на изображении может быть разным

## виды возможных нарезок изображения на картинки
- ровные прямоугольники (для картинок, имеющих одинаковые ширину и высоту в пределах изображения)
- картинки различной ширины, но, в пределах строки имеющих одинаковую высоту
- размеры каждой из картинок различны (мозаика)

На данный момент реализована только первая разбивка.
Для разбивки изображения других видов необходимо придумать логику.

Второй вид - возможно построение гистограммы различий цветов по каждой строке, и, нахождения нулей в гистограмме, или же выдающихся пиков - так можно определить строки с картинками. Аналогично поступить для каддой строки и найти вертикальные разделители.

Третий вид - как правило есть фон. Его можно определить как наиболее доминантный цвет. Отдельные картинки определять, вырезать и заменять на цвет фона до тех пор, пока все изображение не станет цвета фона. Находить сканированием цвета, и, поиском путей обхода по периметру.

## распознавание
- используется аплоад на внешний хостинг
- получение внешней ссылки на загруженную картинку
- передача её в image.google
- парсинг страницы с ответом.

Со страницы парсинг выполняется способом:
- английские слова переводятся
- все слова нормализуются (ед.ч, муж.род, именит.падеж)
- выполняется проверка орфографии
- оставшиеся слова ранжируются по частоте

Полученный текст передается далее прочим модулям для обработки.