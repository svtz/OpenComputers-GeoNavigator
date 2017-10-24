local event = require "event"
local component = require "component"
local serialization = require "serialization"
local internet = require "internet"
local serverUrl = "http://svtz.ru:7777/api/values"
local geo = component.geolyzer
local computer = component.computer
local chunkSize = 16
local scanSize = 64
local step = 4
local antiNoise = 4

local char_space = string.byte(" ")
local char_f = string.byte("f")
local running = true -- state variable so the loop can terminate

function unknownEvent()
  -- do nothing if the event wasn't relevant
end

-- table that holds all event handlers
local myEventHandlers = setmetatable({}, { __index = function() return unknownEvent end })

-- Terminate handler
function myEventHandlers.key_up(adress, char, code, playerName)
  if (char == char_space) then
    running = false
  end
  if (char == char_f) then
    print('Next scan will be full.')
    step = 1
  end
end

function send(request)
  local sendImpl = function()
    local response = internet.request(serverUrl, request, { ["Content-Type"] = "application/json; charset=utf-8" })
    for chunk in response do
    end
  end
  local status, err = pcall(sendImpl)
  if not status then
    print(err)
  end
end

function sendPoints(startX, startY, startZ, step, values)
  local requestPattern = [[{"StartX":"%d","StartY":"%d","StartZ":"%d","Step":"%d","hardnessValues":%s}]]
  print("Uploading...")
  local valuesString = "[" .. table.concat(values, ",") .. "]"
  local request = string.format(requestPattern, startX, startY, startZ, step, valuesString)
  send(request)
end

function getHardness(scanResults, y)
  local threshold = 0.2
  if (antiNoise == 1) then return true, scanResults[1][y] end
  if (antiNoise == 2) then
    if math.abs(scanResults[1][y] - scanResults[2][y]) >= threshold then return false end
    return true, (scanResults[1][y] + scanResults[2][y]) / 2
  end
  if (antiNoise >= 3) then
    local median = 0
    for i = 1, antiNoise do
      median = median + scanResults[i][y]
    end
    median = median / antiNoise

    local goodValues = {}
    for i = 1, antiNoise do
      if math.abs(median - scanResults[i][y]) < threshold then
        table.insert(goodValues, scanResults[i][y])
      end
    end
    if #goodValues < math.ceil( antiNoise/2 ) then return false end
    local hardness = 0
    for i = 1, #goodValues do
      hardness = hardness + goodValues[i]
    end
    hardness = hardness / #goodValues
    return true, hardness
  end
end

function performScan(x, z)
  local scanResults = {}
  for i = 1, antiNoise do
    scanResults[i] = geo.scan(x, z, 0, 0, 0, 0, true)
  end
  os.sleep(0)
  return scanResults
end

function scan(x,y,z)
  local scanResult = {}
  local deltaX = math.floor(x/chunkSize) * chunkSize - x
  local deltaZ = math.floor(z/chunkSize) * chunkSize - z

  if (-deltaX < math.floor(chunkSize/2)-1) or
     (-deltaX > math.floor(chunkSize/2)+1) or
     (-deltaZ < math.floor(chunkSize/2)-1) or
     (-deltaZ > math.floor(chunkSize/2)+1) then
    print('Please, stay closer to the chunk center. You current offset is ('..deltaX..', '..deltaZ..')')
    return
  end
  cx = deltaX
  while cx < deltaX+chunkSize do
    cz = deltaZ
    while cz < deltaZ+chunkSize do
      io.write("Scanning at (" .. cx-deltaX+1 .. ", " .. cz-deltaZ+1 .. ")")
      local scanResults = performScan(cx, cz)

      for cy = 1, scanSize do
        local posY = y - cy
        if posY <= 0 then break end

        while true do
          local hardnessState, hardnessValue = getHardness(scanResults, cy)
          if hardnessState then 
            table.insert(scanResult, hardnessValue)
            break
          end
          io.write(".")
          scanResults = performScan(cx, cz)
        end
      end

      cz = cz + step
      io.write("\n")
    end
    sendPoints(x+cx, y, z+deltaZ, step, scanResult)
    scanResult = {}
    computer.beep(1000, 0.3)
    os.sleep(0)
    cx = cx + step
  end

  
  step = 4
end

-- Table use handler
function myEventHandlers.tablet_use(data)
  scan(data.posX, data.posY, data.posZ)
  computer.beep(900, 0.3)
  computer.beep(800, 0.3)
  computer.beep(750, 0.3)
  computer.beep(660, 0.3)
  computer.beep(600, 0.5)
  print("Complete.")
end

-- The main event handler (entry)
function handleEvent(eventID, ...)
  if (eventID) then -- can be nil if no event was pulled for some time
    myEventHandlers[eventID](...)
  end
end

-- main event loop which processes all events, or sleeps if there is nothing to do
print("Press <space> to exit.")
while running do
  handleEvent(event.pull()) -- sleeps until an event is available, then process it
end
