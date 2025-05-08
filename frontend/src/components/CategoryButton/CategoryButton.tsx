import { ButtonBase, SxProps, Theme } from "@mui/material";

type CategoryButtonProps = {
    label: string;
    isSelected: boolean;
    onClick: () => void;
    sx?: SxProps<Theme>;
};

export function CategoryButton({ label, isSelected, onClick, sx }: CategoryButtonProps) {
    return (
      <ButtonBase
        onClick={onClick}
        sx={{
          border: "1px solid",
          borderColor: "var(--accent-color)",
          backgroundColor: isSelected ? "var(--accent-color)" : "transparent",
          color: isSelected ? "white" : "var(--accent-color)",
          borderRadius: 2,
          px: 2,
          py: 1,
          transition: "all 0.3s",
          "&:hover": {
            backgroundColor: isSelected ? "primary.dark" : "grey.100",
          },
          ...sx,
        }}
      >
        {label}
      </ButtonBase>
    );
  }