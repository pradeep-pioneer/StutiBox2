import * as React from 'react'
interface IMediaItemsGridProps {
    alarmTime?: string
    alarmMissThreshold?: string
    alarmAutoTurnOffCheckTime: string
    enabled: boolean
}
const AlarmControl = (props: IMediaItemsGridProps) => {
    return (
        <div>
            <h1>Alarm</h1>
            <form>
                <label>{props.alarmTime}</label>
                <label>{props.alarmMissThreshold}</label>
                <label>{props.alarmAutoTurnOffCheckTime}</label>
                <input type="checkbox" id="vehicle1" name="vehicle1" value="Bike"/>
            </form>
        </div>
    )
}
export default AlarmControl;