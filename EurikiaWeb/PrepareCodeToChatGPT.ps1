# Configura el directorio del proyecto y el tipo de archivos a incluir
$projectDirectory = "C:\ID\OneDrive\Eukairia\EurikiaWeb"
$fileTypes = @("*.cs", "*.razor") # Añade o quita tipos de archivos según sea necesario

# Nombre del archivo de salida combinado
$combinedFile = "C:\ID\OneDrive\Eukairia\EurikiaWeb\proyecto-completo.txt"

# Combina los archivos
Get-ChildItem -Path $projectDirectory -Include $fileTypes -Recurse | 
Get-Content | Set-Content $combinedFile

# Ahora, divide el archivo combinado en partes de 5MB
$outputFilePrefix = "proyecto-part-"
$partSize = 5MB

# Abre el archivo combinado para leer
$stream = [System.IO.File]::OpenRead($combinedFile)
$buffer = New-Object Byte[] $partSize
$index = 0

while ($readLen = $stream.Read($buffer, 0, $buffer.Length)) {
    $partName = "{0}{1:D4}.txt" -f $outputFilePrefix, ++$index
    [System.IO.File]::WriteAllBytes($partName, $buffer[0..($readLen - 1)])
}

# Cierra el archivo de entrada
$stream.Close()

# Informa al usuario
Write-Host "Proceso completado. Los archivos han sido divididos en partes de 5MB."
