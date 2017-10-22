local event = require "event"
local component = require "component"
local serialization = require "serialization"
local internet = require "internet"
local serverUrl = "http://svtz.ru:7777/api/values"

local char_space = string.byte(" ")
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

function sendPoints(points)
  local pointPattern = [[{"posX":"%d","posZ":"%d","posY":"%d","hardness":"%f"}]]
  local request = ""
  for k,v in pairs(points) do
    local pointString = string.format(pointPattern, v.posX, v.posZ, v.posY, v.hardness)
    if request ~= "" then
      request = request .. "," .. pointString
    else
      request = pointString
    end
  end
  request = "[" .. request .. "]"
  send(request)
end

-- Table use handler
function myEventHandlers.tablet_use(data)
  local points = { data, data }
  sendPoints(points)
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
