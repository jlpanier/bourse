echo Purging %FILE_TO_INSTALL%

rmdir /s /q C:\Projets\Boursorama\Bourse\Bourse\bin
rmdir /s /q C:\Projets\Boursorama\Bourse\Bourse\obj

rmdir /s /q C:\Projets\Boursorama\Business\Bourse\bin
rmdir /s /q C:\Projets\Boursorama\Business\Bourse\obj

rmdir /s /q C:\Projets\Boursorama\Repository\Bourse\bin
rmdir /s /q C:\Projets\Boursorama\Repository\Bourse\obj

del /q *\Resources\Resource.designer.cs
