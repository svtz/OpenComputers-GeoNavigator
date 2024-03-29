local event = require "event"
local component = require "component"
local internet = require "internet"

local computer = component.computer
local term = require "term"
local colors = require "colors"
local gpu = component.gpu

local function readAll(file)
    local f = assert(io.open(file, "r"))
    local content = f:read("*all")
    f:close()
    return content
end

local serverUrl = readAll('server-url'):sub(1, -2)

local char_space = string.byte(" ")
local char_x = string.byte("x")
local char_s = string.byte("s")
local char_f = string.byte("f")
local char_i = string.byte("i")

local running = true
local isInteractive = true

local states = {
  sendBlock = "sendBlock",
  find = "find",
  info = "info"
}
local state

local function writeWithColor(color, text)
  local oldColor = gpu.setForeground(color, true)
  term.write(text)
  return oldColor
end

local function changeState(newState)
  if (newState == states.sendBlock) then
    local originalForeground = writeWithColor(colors.yellow, 'Scan ')
    writeWithColor(colors.green, 'mode activated. Use the tablet to scan ores.\n')
    gpu.setForeground(originalForeground)
  end
  if (newState == states.find) then
    local originalForeground = writeWithColor(colors.yellow, 'Find ')
    writeWithColor(colors.green, 'mode activated. Use the tablet to select a point for the search to begin.\n')
    gpu.setForeground(originalForeground)
  end
  if (newState == states.info) then
    local originalForeground = writeWithColor(colors.yellow, 'Info ')
    writeWithColor(colors.green, 'mode activated. Use the tablet to select a point for the search to begin.\n')
    gpu.setForeground(originalForeground)
  end

  state = newState
end

local dimensions = {
  "Overworld",
  "Nether",
  "End",
  "Moon",
  "Mars",
  "Asteroids"
}
local currentDimension = dimensions[1]

local ores = {
  Apatite = "Apatite",
  Almandine = "Almandine",
  Aluminium = "Aluminium",
  BandedIron = "BandedIron",
  Bastnasite = "Bastnasite",
  Barite = "Barite",
  Bauxite = "Bauxite",
  Bentonite = "Bentonite",
  Beryllium = "Beryllium",
  BrownLimonite = "BrownLimonite",
  Calcite = "Calcite",
  Cassiterite = "Cassiterite",
  CertusQuartz = "CertusQuartz",
  Chalcopyrite = "Chalcopyrite",
  Cinnabar = "Cinnabar",
  Coal = "Coal",
  Cobaltite = "Cobaltite",
  Copper = "Copper",
  Diamond = "Diamond",
  Emerald = "Emerald",
  EnrichedNaquadah = "EnrichedNaquadah",
  Galena = "Galena",
  Garnierite = "Garnierite",
  Glauconite = "Glauconite",
  Gold = "Gold",
  Graphite = "Graphite",
  Grossular = "Grossular",
  Ilmenite = "Ilmenite",
  Iridium = "Iridium",
  Iron = "Iron",
  Lapis = "Lapis",
  Lazurite = "Lazurite",
  Lead = "Lead",
  Lepidolite = "Lepidolite",
  Lignite = "Lignite",
  Lithium = "Lithium",
  Magnesite = "Magnesite",
  Magnetite = "Magnetite",
  Malachite = "Malachite",
  Manganese = "Manganese",
  GreenSapphire = "GreenSapphire",
  Molybdenite = "Molybdenite",
  Molybdenum = "Molybdenum",
  Monazite = "Monazite",
  Naquadah = "Naquadah",
  Neodymium = "Neodymium",
  NetherQuartz = "NetherQuartz",
  Nickel = "Nickel",
  Olivine = "Olivine",
  Palladium = "Palladium",
  Pentlandite = "Pentlandite",
  Phosphate = "Phosphate",
  Phosphorus = "Phosphorus",
  Pitchblende = "Pitchblende",
  Platinum = "Platinum",
  OilSands = "OilSands",
  Powellite = "Powellite",
  Pyrite = "Pyrite",
  Pyrolusite = "Pyrolusite",
  Pyrope = "Pyrope",
  Quartz = "Quartz",
  Quartzite = "Quartzite",
  Redstone = "Redstone",
  RockSalt = "RockSalt",
  Ruby = "Ruby",
  Salt = "Salt",
  Sapphire = "Sapphire",
  Scheelite = "Scheelite",
  Sheldonite = "Sheldonite",
  Silver = "Silver",
  Soapstone = "Soapstone",
  Sodalite = "Sodalite",
  Spessartine = "Spessartine",
  Sphalerite = "Sphalerite",
  Spodumene = "Spodumene",
  Stibnite = "Stibnite",
  Sulflur = "Sulflur",
  Talc = "Talc",
  Tantalite = "Tantalite",
  Tetrahedrite = "Tetrahedrite",
  Thorium = "Thorium",
  Tin = "Tin",
  Tungstate = "Tungstate",
  Uraninite = "Uraninite",
  Uranium238 = "Uranium238",
  VanadiumMagnetite = "VanadiumMagnetite",
  Wulfenite = "Wulfenite",
  YellowLimonite = "YellowLimonite"
}

function unknownEvent()
  -- do nothing if the event wasn't relevant
end

-- table that holds all event handlers
local myEventHandlers = setmetatable({}, { __index = function() return unknownEvent end })

local function doRequest(uri, requestBody)
  isInteractive = false
  local sendImpl = function()
    local headers = requestBody and { ["Content-Type"] = "application/json; charset=utf-8" } or nil
    local response = internet.request(serverUrl .. uri, requestBody, headers)
    local oldColor = writeWithColor(colors.gray, "> ")
    for chunk in response do
      term.write((not (chunk == nil)) and chunk or 'nil', true)
    end
    gpu.setForeground(oldColor)
  end
  local status, err = pcall(sendImpl)
  if not status then
    print(err)
  end
  isInteractive = true
end

local function getOreCandidates(line, pos)
  local result = {}
  for k,v in pairs(ores) do
    if (string.sub(v,1,string.len(line))==line) then
      table.insert(result, v)
    end
  end
  return result
end
local function getNameFromUser()
  term.write('> ')
  isInteractive = false
  local result = term.read(nil, true, getOreCandidates):sub(1, -2)
  isInteractive = true
  return result
end

local function oreNameIsValid(oreName)
  for k,v in pairs(ores) do
    if v == oreName then
      return true
    end
  end
  return false
end

local function errorSound()
  computer.beep(800, 0.2)
  computer.beep(800, 0.2)
end

local function successSound()
  computer.beep(1500, 0.3)
end

local function sendBlock(data)
  local originalForeground = writeWithColor(colors.green, 'Block ')
  writeWithColor(colors.yellow, '('..data.posX..', '..data.posY..', '..data.posZ..')')
  writeWithColor(colors.green, ' at ')
  writeWithColor(colors.yellow, currentDimension)
  writeWithColor(colors.green, '.\nPlease, enther the ore name:\n')
  gpu.setForeground(originalForeground)
  
  local oreName = getNameFromUser()
  if (oreNameIsValid(oreName)) then
    local requestPattern = [[{"PosX":"%d","PosY":"%d","PosZ":"%d","Ore":"%s","Dimension":"%s"}]]
    local request = string.format(requestPattern, data.posX, data.posY, data.posZ, oreName, currentDimension)
    doRequest('blocks/add', request)
    local originalForeground = writeWithColor(colors.green, 'Block saved\n')
    gpu.setForeground(originalForeground)
    successSound()
    changeState(states.sendBlock)
    return
  end
  errorSound()
  local originalForeground = writeWithColor(colors.red, 'Ore is invalid\n')
  gpu.setForeground(originalForeground)
  changeState(states.sendBlock)
end

local function find(data)
  local originalForeground = writeWithColor(colors.green, 'Enter the ore to find:\n')
  gpu.setForeground(originalForeground)
  local oreName = getNameFromUser()
  if (not oreNameIsValid(oreName)) then
    errorSound()
    local originalForeground = writeWithColor(colors.red, 'Ore is invalid\n')
    gpu.setForeground(originalForeground)
  else
    local originalForeground = writeWithColor(colors.green, 'Search results:\n')
    doRequest('blocks/find?dimension='..currentDimension..'&ore='..oreName..'&x='..data.posX..'&y='..data.posY..'&z='..data.posZ..'&limit=5')
    gpu.setForeground(originalForeground)
  end

  changeState(states.find)
end

local function info()
  local originalForeground = writeWithColor(colors.yellow, 'Info ')
  writeWithColor(colors.green, 'mode. Enter the ore to view the vein info:\n')
  gpu.setForeground(originalForeground)
  local oreName = getNameFromUser()
  if (not oreNameIsValid(oreName)) then
    errorSound()
    local originalForeground = writeWithColor(colors.red, 'Ore is invalid\n')
    gpu.setForeground(originalForeground)
  else
    local originalForeground = writeWithColor(colors.green, 'Search results:\n')
    doRequest('veins/info?dimension='..currentDimension..'&ore='..oreName)
    gpu.setForeground(originalForeground)
  end

  changeState(state)
end

local function switchDimension()
  for k,v in pairs(dimensions) do
    if (v == currentDimension) then
      currentDimension = dimensions[k+1]
      if (currentDimension == nil) then
        currentDimension = dimensions[1]
      end
      local originalForeground = writeWithColor(colors.green, 'Current dimension changed to ')
      writeWithColor(colors.yellow, currentDimension)
      writeWithColor(colors.green, '.\n')
      gpu.setForeground(originalForeground)
      return
    end
  end
end

-- Table use handler
function myEventHandlers.tablet_use(data)
  if (state == states.sendBlock) then
    sendBlock(data)
    return
  end
  if (state == states.find) then
    find(data)
    return
  end
end


function myEventHandlers.key_up(adress, char, code, playerName)
  if (not isInteractive) then
    return
  end

  if (char == char_space) then
    print('Exiting')
    running = false
    return
  end

  if (char == char_x) then
    switchDimension()
    return
  end

  if (char == char_s) then
    changeState(states.sendBlock)
    return
  end

  if (char == char_f) then
    changeState(states.find)
    return
  end

  if (char == char_i) then
    info()
    return
  end
end

-- The main event handler (entry)
function handleEvent(eventID, ...)
  if (eventID) then -- can be nil if no event was pulled for some time
    myEventHandlers[eventID](...)
  end
end

-- main event loop which processes all events, or sleeps if there is nothing to do
print("Press <space> to exit.")
changeState(states.sendBlock)
while running do
  handleEvent(event.pull()) -- sleeps until an event is available, then process it
end
