import { StepIconProps, styled } from "@mui/material";

export default function QontoStepIcon(props: StepIconProps) {
    const { active, completed, className } = props;
  
    const QontoStepIconRoot = styled('div')<{ ownerState: { active?: boolean } }>(
        ( ) => ({
          color: 'black',
          display: 'flex',
          height: 22,
          alignItems: 'center',
          '& .QontoStepIcon-completedIcon': {
            color: 'var(--accent-color)',
            zIndex: 1,
            fontSize: 18,
          },
          '& .QontoStepIcon-outer-circle': {
            display: 'flex',
            justifyContent: 'center',
            alignItems: 'center',
            width: 28,
            height: 28,
            borderRadius: '50%',
            backgroundColor: '#ccc',
            zIndex: 1
          },
          '& .QontoStepIcon-inner-circle': {
            width: 10,
            height: 10,
            borderRadius: '50%',
            backgroundColor: 'white',
            zIndex: 0
          },
          '& .QontoStepIcon-inner-circle.active': {
            backgroundColor: 'var(--accent-color)',
          },
        }),
      );      

    return (
      <QontoStepIconRoot ownerState={{ active }} className={className}>
        {completed ? (
          <div className="QontoStepIcon-outer-circle">
            <div className="QontoStepIcon-inner-circle active"></div>
          </div>
          
        ) : (
          <div className="QontoStepIcon-outer-circle">
            <div className="QontoStepIcon-inner-circle"></div>
          </div>
        )}
      </QontoStepIconRoot>
    );
  }